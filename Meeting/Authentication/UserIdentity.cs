using Meeting.Models;
using Meeting.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Meeting.Authentication
{
    public class UserIndentity : IIdentity, IUserProvider
    {
        public User User
        {
            get;
            set;
        }

        public string AuthenticationType
        {
            get
            {
                return typeof(User).ToString();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return User != null;
            }
        }

        public string Name
        {
            get
            {
                if (User != null)
                {
                    return User.Nick;
                }
                //иначе анон
                return "anonym";
            }
        }



        public void Init(string nickAndChatID, MeetingContainer model)
        {
            User = UserRepository.GetUserByStringWithNickAndChatID(nickAndChatID, model);
        }
    }
}