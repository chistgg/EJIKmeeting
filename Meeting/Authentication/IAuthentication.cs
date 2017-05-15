using Meeting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Principal;

namespace Meeting.Authentication
{
    public interface IAuthentication
    {
        HttpContext HttpContext { get; set; }

        User Login(string login, string password, string chatID, bool isPersistent);

        User Login(string hashedUserID);

        void LogOut();

        IPrincipal CurrentUser { get; }
    }
}
