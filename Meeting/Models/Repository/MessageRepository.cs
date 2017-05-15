using Meeting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meeting.Models.Repository
{
    public static class MessageRepository
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void AddMessage(Message newMessage, MeetingContainer model)
        {
            newMessage.ID = Guid.NewGuid();
            model.MessageSet.Add(newMessage);

            model.SaveChanges();
        }

        public static void AddMessage(Guid userID, string content, DateTime sendingTime, bool toProtocol, MeetingContainer model)
        {
            var newMessage = new Message
            {
                ID = Guid.NewGuid(),
                User = UserRepository.GetUserByID(userID, model),
                File = null,
                Content = content,
                SendingTime = sendingTime,
                Status = toProtocol ? "1001" : "1000"
            };

            model.MessageSet.Add(newMessage);

            model.SaveChanges();

        }

        public static void AddMessageWithFile(Guid userID, string content, DateTime sendingTime, string fileName, Guid fileID, bool toProtocol, MeetingContainer model)
        {
            var newMessage = new Message
            {
                ID = Guid.NewGuid(),
                User = UserRepository.GetUserByID(userID, model),
                File = GetNewFile(fileName, fileID),
                Content = content,
                SendingTime = sendingTime,
                Status = toProtocol ? "1001" : "1000"
        };

            model.MessageSet.Add(newMessage);

            model.SaveChanges();
        }

        public static Models.File GetNewFile(string fileName, Guid fileID)
        {
            try
            {
                return new Models.File
                {
                    ID = fileID,
                    Name = fileName
                };
            }
            catch (Exception ex)
            {
                logger.Error("Не удалось получить объект класса File для пути " + fileID.ToString() + " " + ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public static List<Message> GetAllMessages(Guid chatID, MeetingContainer model)
        {
            return model.MessageSet
                .Where(m => m.User.Chat.ID.Equals(chatID))
                .OrderBy(m => m.SendingTime)
                .ToList();
        }

        public static List<Message> GetAllMessages(Chat chat)
        {
            return chat.User
                .SelectMany(u => u.Message)
                .OrderBy(m => m.SendingTime)
                .ToList();
        }

        public static List<Message> GetInProtocolMessages(List<Message> messages)
        {
            return messages
                .Where(m => MessageStatusHelper.IsMessageInProtocol(m.Status))
                .OrderBy(m => m.SendingTime)
                .ToList();
        }

        public static List<Message> GetInProtocolMessages(Guid chatID, MeetingContainer model)
        {
            var messages = GetAllMessages(chatID, model);

            return messages
                .Where(m => MessageStatusHelper.IsMessageInProtocol(m.Status))
                .ToList();      //TODO исправить: метод вызывается дважды.
        }

        public static List<Message> GetLastMessagesPerPage(Guid chatID, MeetingContainer model, int messagesPerPage = 50, int numOfPage = 1)
        {
            int numOfAlreadyReadMessages = messagesPerPage * (numOfPage - 1);

            return model.MessageSet
                .Where(m => m.User.Chat.ID.Equals(chatID))
                .OrderByDescending(m => m.SendingTime)
                .Skip(numOfAlreadyReadMessages)
                .Take(messagesPerPage)
                .ToList();
        }

        public static List<Message> GetLastMessagesPerPage(Guid chatID, int messagesPerPage = 50, int numOfPage = 1)
        {
            MeetingContainer model = new MeetingContainer();
            int numOfAlreadyReadMessages = messagesPerPage * (numOfPage - 1);

            return model.MessageSet
                .Where(m => m.User.Chat.ID.Equals(chatID))
                .OrderByDescending(m => m.SendingTime)
                .Skip(numOfAlreadyReadMessages)
                .Take(messagesPerPage)
                .ToList();
        }

        public static DateTime GetLastMessageDateTime(List<Message> message)
        {
            DateTime latestDT = message.First().SendingTime;

            foreach (var m in message)
            {
                if (m.SendingTime.CompareTo(latestDT) < 0)
                    latestDT = m.SendingTime;
            }

            return latestDT;
        }

        public static void AddMessageToProtocol(Guid messageID, MeetingContainer model)
        {
            var message = model.MessageSet.Where(m => m.ID.Equals(messageID)).First();

            message.Status = StringHelper.ReplaceCharInString(message.Status, 3, '1');

            model.SaveChanges();
        }

        public static void RemoveMessageFromProtocol(Guid messageID, MeetingContainer model)
        {
            var message = model.MessageSet.Where(m => m.ID.Equals(messageID)).First();

            message.Status = StringHelper.ReplaceCharInString(message.Status, 3, '0');

            model.SaveChanges();
        }

        public static void AddAllMessagesToProtocol(Guid chatID, MeetingContainer model)
        {
            var messages = GetAllMessages(chatID, model);

            for (int i = 0; i < messages.Count; i++)
            {
                var message = messages[i];
                message.Status = StringHelper.ReplaceCharInString(message.Status, 3, '1');
            }
            model.SaveChanges();
        }

        public static void RemoveAllMessagesFromProtocol(Guid chatID, MeetingContainer model)
        {
            var messages = GetAllMessages(chatID, model);

            for (int i = 0; i < messages.Count; i++)
            {
                var message = messages[i];
                message.Status = StringHelper.ReplaceCharInString(message.Status, 3, '0');
            }
            model.SaveChanges();
        }

        public static bool AreAllMessagesInProtocol(Chat chat)
        {
            var messages = GetAllMessages(chat);

            if (messages.All(m => m.Status[3] == '1'))
                return true;
            else
                return false;
        }
    }
}