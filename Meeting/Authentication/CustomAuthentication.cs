using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Meeting.Models;
using Ninject;
using System.Security.Principal;
using Meeting.Models.Repository;
using System.Web.Security;
using Meeting.Helpers;

namespace Meeting.Authentication
{
    public class CustomAuthentication : IAuthentication
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string COOKIE_NAME = "^__^AUTH_COOKIE";

        public HttpContext HttpContext
        {
            get;
            set;
        }

        [Inject]
        public MeetingContainer Model { get; set; }

        public User Login(string hashedUserID)
        {
            User user = UserRepository.GetUserByHashedID(hashedUserID, Model);

            if (user != null)
            {
                string loginForAuth = user.Nick + " " + user.Chat.ID.ToString();
                CreateCookie(loginForAuth);
            }
            return user;
        }

        public User Login(string nick, string password, string chatID, bool isPersistent)
        {
            Guid chatIDGuid = StringHelper.StrToGuidOrEmptGuid(chatID);
           
            User user = UserRepository.LoginUser(nick, password, chatIDGuid, Model);
            string loginForAuth = nick + " " + chatID;

            if (user != null)
            {
                CreateCookie(loginForAuth, isPersistent);
            }
            return user;
        }


        
        private void CreateCookie(string userName, bool isPersistent = false)
        {
            var ticket = new FormsAuthenticationTicket(
                  1,
                  userName,
                  DateTime.Now,
                  DateTime.Now.Add(FormsAuthentication.Timeout),
                  isPersistent,
                  string.Empty,
                  FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            var encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            var authCookie = new HttpCookie(COOKIE_NAME) //domain is null
            {
               // Domain = FormsAuthentication.CookieDomain,
                Value = encTicket,
                Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
            };

          //  var cookiesInResponce = HttpContext.Response.Cookies;

            HttpContext.Response.Cookies.Set(authCookie);
        }

        public void LogOut()
        {
            var httpCookie = HttpContext.Response.Cookies[COOKIE_NAME];
            if (httpCookie != null)
            {
                httpCookie.Value = string.Empty;
            }
        }

        private IPrincipal _currentUser;

        public IPrincipal CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    try
                    {
                        HttpCookie authCookie = HttpContext.Request.Cookies.Get(COOKIE_NAME);
                        if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
                        {
                            var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                            _currentUser = new UserProvider(ticket.Name, Model);
                        }
                        else
                        {
                            _currentUser = new UserProvider(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Failed authentication: " + ex.InnerException.Message.ToString()+ " Stack Trace: " + ex.InnerException.StackTrace.ToString());
                        _currentUser = new UserProvider(null, null);
                    }
                }
                return _currentUser;
            }
        }

    }
}