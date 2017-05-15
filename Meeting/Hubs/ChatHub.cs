using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Meeting.Models.ChatModels;
using Meeting.Models;
using Ninject;
using Meeting.Authentication;
using Meeting.Models.Repository;
using System.Threading.Tasks;
using Meeting.Helpers;
using System.Collections;

namespace Meeting.Hubs
{
    public class ChatHub : Hub
    {
       public static MeetingContainer Model = new MeetingContainer();

       private static List<ChatUser> Users = new List<ChatUser>();
       private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Send(string connectionID, string content, string fileID = "", string fileName = "", bool toProtocol = false)
        {
            if (content.Length <= 0)
                return;

            var sender = Users.Where(u => u.ConnectionID.Equals(connectionID)).First();

            if (TryToExecuteChatCommand(sender, content))
                return;

            SendMessage(sender, content, fileID, fileName, toProtocol);

        }

        public delegate void SendComand(ChatUser chatUser);
        public bool TryToExecuteChatCommand(ChatUser chatUser, string content)
        {
            var senderCommand = GetChatComand(content, UserStatusHelper.IsAdmin(chatUser.Status));

            if (senderCommand != null)
            {
                senderCommand(chatUser);
                return true;
            }

            return false;
        }
        public SendComand GetChatComand(string content, bool isUserAdmin)
        {
            SendComand sendCommand = null;

            if (isUserAdmin)
                sendCommand = GetAdminComand(content);

            if (sendCommand == null)
                sendCommand = GetVoteComand(content);

            return sendCommand;
        }

        public SendComand GetAdminComand(string content)
        {
            switch (content)
            {
                case "_stopchat":
                    return StopChat;  
                case "_activatechat":
                    return ActivateChat;
                case "_sendprotocol":
                    return SendProtocol;
                default: return null;
            }
        }

        public SendComand GetVoteComand(string content)
        {
            switch (content)
            {
                case "_confirm":
                    return ConfirmProtocol;
                case "_reject":
                    return RejectProtocol;
                default: return null;
            }
        }

        public void StopChat(ChatUser sender)
        {
            Clients.Group(sender.ChatID.ToString()).onChatStop();
            ChatRepository.ChatAborted(sender.ChatID, Model);
        }

        public void ActivateChat(ChatUser sender)
        {
            Clients.Group(sender.ChatID.ToString()).onChatContinue();
            ChatRepository.ChatActivate(sender.ChatID, Model);
        }

        public void SendProtocol(ChatUser sender) //Форма голосования показывается у всех юзеров, в том числе у админа и тех, кто не писал сообщений.
        {
            ChatRepository.SetNewProtocolConformation(sender.ChatID, Model);
            Clients.Group(sender.ChatID.ToString()).onSendNewProtocol();
        }

        public void ConfirmProtocol(ChatUser sender)
        {
            UserRepository.SetUserStatus_ConfirmProtocol(sender.UserID, Model);
            Clients.Group(sender.ChatID.ToString()).onVote(sender.ConnectionID, "Утвердил протокол");

            if (ChatRepository.TryToConfirmProtocol(sender.ChatID, Model)) 
            {

                Clients.Group(sender.ChatID.ToString()).addMessage("Info", "Протокол согласован.", StringHelper.DateTimeToString(DateTime.Now), "0", "protocol.pdf", false);
                Clients.Group(sender.ChatID.ToString()).onConfirmProtocol();
            }
        }

        public void RejectProtocol(ChatUser sender)
        {
            UserRepository.SetUserStatus_RejectProtocol(sender.UserID, Model);
            Clients.Group(sender.ChatID.ToString()).onVote(sender.ConnectionID, "Отклонил протокол");
        }

        public void SendMessage(ChatUser sender, string content, string fileID, string fileName, bool toProtocol)
        {
            Guid fileIDGuid = StringHelper.StrToGuidOrEmptGuid(fileID);

            if (!fileIDGuid.Equals(Guid.Empty))
            {
                SendMessageWithFile(sender, content, fileIDGuid, fileName, toProtocol);
            }
            else
            {
                SendMessageWithoutFile(sender, content, toProtocol);
            }

            if (!UserStatusHelper.HasSendedMessages(sender.Status))
            {
                SetUserStatus_HasSendedMessages(sender);
            }
        }

        private void SendMessageWithFile(ChatUser sender, string content, Guid fileID, string fileName, bool toProtocol)
        {
            var sendingTime = DateTime.Now;
            Clients.Group(sender.ChatID.ToString()).addMessage(sender.Nick, content, StringHelper.DateTimeToString(sendingTime), fileID.ToString(), fileName, toProtocol);// кодированная ссылка на представителя файла
            MessageRepository.AddMessageWithFile(sender.UserID, content, sendingTime, fileName, fileID, toProtocol, Model);
        }

        private void SendMessageWithoutFile(ChatUser sender, string content, bool toProtocol)
        {
            var sendingTime = DateTime.Now;
            Clients.Group(sender.ChatID.ToString()).addMessage(sender.Nick, content, StringHelper.DateTimeToString(sendingTime), "", "", toProtocol);
            MessageRepository.AddMessage(sender.UserID, content, sendingTime, toProtocol, Model);
        }

        private void SetUserStatus_HasSendedMessages(ChatUser sender)
        {
            sender.Status = StringHelper.ReplaceCharInString(sender.Status, 1, '2');
            UserRepository.SetUserStatus_HasSendedMessages(sender.UserID, Model);
        }

        // Подключение нового пользователя
        public async Task Connect(string userID, string chatID)
        {
            var id = Context.ConnectionId;
            Guid userIDGuid = StringHelper.StrToGuidOrEmptGuid(userID);

            if (!Users.Any(x => x.ConnectionID == id) && !IsUserExist(userIDGuid))  
            {
               var chatUser = new ChatUser(id, userIDGuid);
               Users.Add(chatUser);

                await Groups.Add(Context.ConnectionId, chatID);

                // Посылаем сообщение текущему пользователю
                var userSet = Users.Select(u => new { u.ConnectionID, u.Nick, u.Status });

                if (ChatRepository.CheckProtocolInConfirmation(chatUser.ChatID, Model)
                    && UserStatusHelper.HasSendedMessages(chatUser.Status)
                    && !UserStatusHelper.DidVoteForProtocol(chatUser.Status))
                         Clients.Caller.onConnected(id, chatUser.Nick, true, userSet);
                else
                    Clients.Caller.onConnected(id, chatUser.Nick, false, userSet);

                // Посылаем сообщение всем пользователям !в группе!, кроме текущего
                Clients.Group(chatID).onNewUserConnected(id, chatUser.Nick, chatUser.Status);
                logger.Info("В группу" + chatID + "добавился юзер: " + chatUser.Nick);

                AddLastMessagesToNewUser(id, chatID);
            }
        }

        public bool IsUserExist(Guid userID)
        {
            return Users.Any(u => u.UserID.Equals(userID)) ? true : false; //не позволит пользователю сидеть с разных устройств/браузеров/вкладок

        }

        public ArrayList GetSimpleUserSet(IEnumerable<ChatUser> users)
        {
            var colect = users.Select(u => new { u.ConnectionID, u.Nick });
            var userSet = new ArrayList(colect.ToList());

            return userSet;
        }

        private void AddLastMessagesToNewUser(string id, string chatID)
        {
            var messages = MessageRepository.GetLastMessagesPerPage(Guid.Parse(chatID), Model).OrderBy(m => m.SendingTime);

            foreach (var m in messages)
            {
                if (m.File != null)
                    Clients.Caller.addMessage(m.User.Nick, m.Content, m.SendingTime.ToShortDateString() + " " + m.SendingTime.ToShortTimeString(), m.File.ID.ToString(), m.File.Name, MessageStatusHelper.IsMessageInProtocol(m.Status));
                else
                    Clients.Caller.addMessage(m.User.Nick, m.Content, m.SendingTime.ToShortDateString() + " " + m.SendingTime.ToShortTimeString(),"", "", MessageStatusHelper.IsMessageInProtocol(m.Status));
            }   
        }


        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
         {
             var chatUser = Users.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId);
            if (chatUser != null)
            {
                Users.Remove(chatUser);
                var id = Context.ConnectionId;
                Clients.Group(chatUser.ChatID.ToString()).onUserDisconnected(id, chatUser.Nick);
            }   
 
            return base.OnDisconnected(stopCalled);
        }


    }
}