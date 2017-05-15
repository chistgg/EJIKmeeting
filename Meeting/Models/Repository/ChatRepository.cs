using Meeting.Helpers;
using Meeting.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Helpers;

namespace Meeting.Models.Repository
{
    public static class ChatRepository
    {
        public static Chat GetChatByID(Guid chatID, MeetingContainer model)
        {
            return (from c in model.ChatSet
                    where c.ID == chatID
                    select c).First();
        }

        public static Chat GetChatByHashedID(string hashedChatID, MeetingContainer model)
        {
            return (from c in model.ChatSet
                    where Crypto.VerifyHashedPassword(hashedChatID, c.ID.ToString())
                    select c).First();
        }

        public static List<Guid> GetChatIDs(MeetingContainer model)
        {
            return (from c in model.ChatSet
                    select c.ID).ToList();
        }
        public static bool IsChatExist(Guid chatID, MeetingContainer model)
        {
            return model.ChatSet.Any(c => c.ID == chatID);
        }

        public static void ChatAborted(Guid chatID, MeetingContainer model)
        {
            var chat = GetChatByID(chatID, model);

            chat.Status = StringHelper.ReplaceCharInString(chat.Status, 1, '1');
            chat.EndingTime = DateTime.Now;

            model.SaveChanges();
        }

        public static void ChatActivate(Guid chatID, MeetingContainer model)
        {
            var chat = GetChatByID(chatID, model);

            chat.Status = StringHelper.ReplaceCharInString(chat.Status, 1, '0');
            chat.EndingTime = null;

            model.SaveChanges();
        }

        public static bool CheckProtocolInConfirmation(Guid chatID, MeetingContainer model)
        {
            var chat = GetChatByID(chatID, model);

            if (ChatStatusHelper.IsProtocolInConfirming(chat.Status))
                return true;
            else
                return false;
        }

        public static bool CheckFullConfirmation(Guid chatID, MeetingContainer model)
        {
            var chat = GetChatByID(chatID, model);

            if (chat.User.All(u => (u.Status[1] != '0' && u.Status[3] == '1')))
                return true;
            else
                return false;
        }

        public static bool TryToConfirmProtocol(Guid chatID, MeetingContainer model)
        {
            var chat = GetChatByID(chatID, model);

            if (chat.User.All(u => (u.Status[1] != '0' && u.Status[3] == '1')))
            {
                SetChatStatus_FullProtocolConfirmationWithoutSaving(ref chat);
                model.SaveChanges();
                return true;
            }
            else
                return false;
        }

        public static void SendProtocolToEJIK(Chat chat)
        {
            var pairs = new NameValueCollection();


            var webClient = new WebClient();
            webClient.QueryString.Add("format", "json");
         //   var response = webClient.UploadValues("WebServices/MeetingService.asmx/Method_1_POST_Objects", json);
        }

        public class ReportArg
        {
            public Guid ChatID { get; set; }
            public List<Guid> UserIDs { get; set; }
            public Guid AdminID { get; set; }
            public string ChatName { get; set; }

            public ReportArg(Chat chat)
            {
                ChatID = chat.ID;
                ChatName = chat.Name;
                UserIDs = new List<Guid>();
                foreach (var u in chat.User)
                {
                    if (u.EJIK_ID != null && u.EJIK_ID != Guid.Empty)
                        UserIDs.Add((Guid)u.EJIK_ID);
                }

                var AdminEJIKID = (chat.User.Where(u => UserStatusHelper.IsAdmin(u.Status)).First()).EJIK_ID; 
            }
        }

        public static void ResetProtocolConformation(Guid chatID, MeetingContainer model)   
        {
            var chat = GetChatByID(chatID, model);

            SetChatStatus_ResetProtocolConfirmationWithoutSaving(ref chat);
            SetUsersStatus_ResetProtocolConfirmationWithoutSaving(ref chat);

            model.SaveChanges();
        }

        public static void SetNewProtocolConformation(Guid chatID, MeetingContainer model)
        {
            var chat = GetChatByID(chatID, model);

            SetChatStatus_SetNewProtocolConfirmationWithoutSaving(ref chat);
            SetUsersStatus_ResetProtocolConfirmationWithoutSaving(ref chat);

            model.SaveChanges();
        }

        private static void SetChatStatus_FullProtocolConfirmationWithoutSaving(ref Chat chat)
        {
            chat.Status = StringHelper.ReplaceCharInString(chat.Status, 3, '2');
        }

        private static void SetChatStatus_SetNewProtocolConfirmationWithoutSaving(ref Chat chat)
        {
            chat.Status = StringHelper.ReplaceCharInString(chat.Status, 3, '1');
        }

        private static void SetChatStatus_ResetProtocolConfirmationWithoutSaving(ref Chat chat)
        {
            chat.Status = StringHelper.ReplaceCharInString(chat.Status, 3, '0');
        }

        private static void SetUsersStatus_ResetProtocolConfirmationWithoutSaving(ref Chat chat)
        {
            var users = chat.User.ToList();

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                user.Status = StringHelper.ReplaceCharInString(user.Status, 3, '0');
            }
        }
    }

}