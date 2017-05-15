using Meeting.Authentication;
using Meeting.Helpers.Mapping;
using Meeting.Models;
using Meeting.Models.Repository;
using Meeting.Protocol;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Meeting.Controllers
{
    public class BaseController : Controller
    {
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        public MeetingContainer Model { get; set; }

        [Inject]
        public IAuthentication Auth { get; set; }

        [Inject]
        public ICustomMapper Mapper { get; set; }

        [Inject]
        public IProtocolGenerate Protocol { get; set; }

        public User CurrentUser
        {
            get
            {
                return ((UserIndentity)Auth.CurrentUser.Identity).User;
            }
        }

        public string ChatContentPath
        {
            get
            {
                return Server.MapPath("~/Content/" + CurrentUser.Chat.ID.ToString() + "/");
            }
        }

        internal void SetChatInfo()
        {
            Session["ChatName"] = CurrentUser.Chat.Name;
            Session["ChatDescription"] = CurrentUser.Chat.Description;
            Session["ChatStartingTime"] = CurrentUser.Chat.StartingTime;
        }

    }
}