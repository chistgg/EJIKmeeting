using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Meeting.Models;

namespace Meeting.Authentication
{
    public class UserProvider : IPrincipal
    {
        private UserIndentity userIdentity { get; set; }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get
            {
                return userIdentity;
            }
        }

        public bool IsInRole(string role)
        {
            if (userIdentity.User == null)
                return false;

            return InRole(userIdentity.User, role);
        }

        public bool InRole(User user, string role)
        {
            if (user.Status == role)
            {
                return true;
            }
            return false;
        }

        #endregion


        public UserProvider(string nickAndChatID, MeetingContainer model)
        {
            userIdentity = new UserIndentity();
            userIdentity.Init(nickAndChatID, model);
        }


        public override string ToString()
        {
            return userIdentity.Name;
        }
    }
}