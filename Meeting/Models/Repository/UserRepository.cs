using Meeting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Meeting.Models.Repository
{
    public static class UserRepository
    {
        public static bool AddNewUser(User newUser, MeetingContainer model)
        {
            var chatsUsers = model.UserSet.Where(u => u.Chat.ID.Equals(newUser.Chat.ID));
            if (chatsUsers.Any(u => u.Nick == newUser.Nick))
                return false;
            else
            {
                string hashPassword = Crypto.HashPassword(newUser.Password);

                newUser.Password = hashPassword;
                newUser.ID = Guid.NewGuid();
                model.UserSet.Add(newUser);

                model.SaveChanges();

                return true;
            }
        }

        public static User LoginUser(string nick, string password, Guid chatID, MeetingContainer model)
        {
            try
            {
                var chatsUsers = model.UserSet.Where(u => u.Chat.ID.Equals(chatID)).ToList();
                User user = chatsUsers.FirstOrDefault(u => u.Nick.Equals(nick));

                if (Crypto.VerifyHashedPassword(user.Password, password))
                    return user;
                else
                    return null;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static User LoginUser(string hashedUserID, MeetingContainer model)
        {
            try
            {
                return GetUserByHashedID(hashedUserID, model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<User> GetChatUsers(Guid chatID, MeetingContainer model)
        {
            var chat = ChatRepository.GetChatByID(chatID, model);

            return chat.User.ToList();
        }

        public static User GetUserByID(Guid userID, MeetingContainer model)
        {
            return (from u in model.UserSet
                    where u.ID == userID
                    select u).First();
        }

        public static User GetUserByHashedID(string hashedUserID, MeetingContainer model)
        {
            try
            {
                var users = model.UserSet;

                foreach (var u in users)
                {
                    if (Crypto.VerifyHashedPassword(hashedUserID, u.ID.ToString()))
                        return u;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static User GetUserByNickAndChatID(string nick, Guid chatID, MeetingContainer model)
        {
            var chatsUsers = model.UserSet.Where(u => u.Chat.ID.Equals(chatID));
            return chatsUsers.FirstOrDefault(u => u.Nick.Equals(nick));
        }

        public static User GetUserByStringWithNickAndChatID(string nickAndChatID, MeetingContainer model)
        {
            if (!string.IsNullOrWhiteSpace(nickAndChatID))
            {
                nickAndChatID.Split();
                int indexOfSpase = nickAndChatID.LastIndexOf(' ');
                string nick = nickAndChatID.Substring(0, indexOfSpase);
                Guid chatID = Guid.Parse(nickAndChatID.Substring(indexOfSpase + 1));
                return GetUserByNickAndChatID(nick, chatID, model);
            }
            else

                return null;
        }

        public static void SetUserStatus_WasInChat(User user, MeetingContainer model)   //user должн быть из контекста model. Плохой метод
        {
            if (!UserStatusHelper.WasInChat(user.Status))
            {
                user.Status = StringHelper.ReplaceCharInString(user.Status, 1, '1'); ;
                model.SaveChanges();
            }
        }

        public static void SetUserStatus_HasSendedMessages(Guid userID, MeetingContainer model)   //user должн быть из контекста model. Плохой метод
        {
            var user = GetUserByID(userID, model);

            user.Status = StringHelper.ReplaceCharInString(user.Status, 1, '2'); ;
            model.SaveChanges();
        }

        public static void SetUserStatus_CondirmProtocol(User user, MeetingContainer model)   //user должн быть из контекста model. Плохой метод
        {
            user.Status = StringHelper.ReplaceCharInString(user.Status, 3, '1');
            model.SaveChanges();
        }

        public static void SetUserStatus_RejectProtocol(User user, MeetingContainer model)   //user должн быть из контекста model. Плохой метод
        {
            user.Status = StringHelper.ReplaceCharInString(user.Status, 3, '0');
            model.SaveChanges();
        }

        public static void SetUserStatus_ConfirmProtocol(Guid userID, MeetingContainer model)
        {
            var user = GetUserByID(userID, model);

            user.Status = StringHelper.ReplaceCharInString(user.Status, 3, '1');
            model.SaveChanges();
        }

        public static void SetUserStatus_RejectProtocol(Guid userID, MeetingContainer model)
        {
            var user = GetUserByID(userID, model);

            user.Status = StringHelper.ReplaceCharInString(user.Status, 3, '2');
            model.SaveChanges();
        }

        public static void SetUsersStatus_ResetProtocolConfirmation(Guid chatID, MeetingContainer model)
        {
            var users = GetChatUsers(chatID, model);

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                user.Status = StringHelper.ReplaceCharInString(user.Status, 3, '0');
            }
            model.SaveChanges();
        }
    }
}