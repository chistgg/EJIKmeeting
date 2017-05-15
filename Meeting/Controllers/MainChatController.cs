using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Meeting.Models;
using Meeting.Models.Repository;
using Meeting.Helpers;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Meeting.Hubs;
using System.IO;
using Meeting.Models.ViewModels;
using AutoMapper;

namespace Meeting.Controllers
{ 
    public class MainChatController : BaseController
    {
        // GET: MainChat
        [HttpGet]
        public ActionResult Index()
        {
            if (ChatStatusHelper.IsProtocolConfirmed(CurrentUser.Chat.Status))
                return RedirectToAction("ProtocolDownload", "MainChat");

            UserRepository.SetUserStatus_WasInChat(CurrentUser, Model);
            return View(CurrentUser);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "completechat")]
        public ActionResult CompleteChat()
        {
            ChatRepository.ChatAborted(CurrentUser.Chat.ID, Model);
            return View(CurrentUser);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "activatechat")]
        public ActionResult ActivateChat()
        {
            ChatRepository.ChatActivate(CurrentUser.Chat.ID, Model);
            return View(CurrentUser);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "addvideo")]
        public ActionResult AddVideoToChat()
        {
            return View(CurrentUser);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "addpad")]
        public ActionResult AddPadToChat()
        {
            return View(CurrentUser);
        }

        [HttpPost]
        public JsonResult Upload()
        {

            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];

                if (upload.FileName == "protocol.pdf")  //временно
                    return Json("");

                var path = ChatContentPath;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path.Replace('/','\\'));

               Guid fileID = WorkWithFiles.UploadFile(upload, path);

                if (!fileID.Equals(Guid.Empty))
                 return Json(new string[] { fileID.ToString(), upload.FileName});
            }
            return Json("");
            
        }

        public FileResult Download(string id)
        {
            var path = ChatContentPath;
            Guid fileID = Guid.Empty;
            if (!Guid.TryParse(id, out fileID))
                return null;

            var file = FileRepository.GetFileByID(fileID, Model);

            path += id;
            var name = file.Name;
            var type = "application/octet-stream";

            return File(path, type, name);
        }

        public FileResult ProtocolDownload()
        {
            var path = ChatContentPath;

            path += "protocol.pdf";
            var name = "protocol.pdf";
            var type = "application/octet-stream";

            return File(path, type, name);
        }

        public void Delete(string fn)
        {
            var path = ChatContentPath;
            WorkWithFiles.DeleteFile(path + fn);

        }


          public PartialViewResult ProtocolChanging()
          {
              Guid chatID = CurrentUser.Chat.ID;
              var messages = MessageRepository.GetAllMessages(chatID, Model);
              var messageViews = messages.Select(m => new MessageProtocolView(m)).ToList();

              return PartialView("Protocol", messageViews);
          }

        [HttpPost]
        public JsonResult CreateProtocol()  //TODO проверка на ошибки
        {
            try
            {
                WorkWithFiles.CreateDirectoryIfNotExists(ChatContentPath);

                Protocol.InitializeParam(CurrentUser.Chat.ID, ChatContentPath + "protocol.pdf");
                Protocol.GenerateProtocol();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " " + ex.StackTrace);
            }
            return Json("");
        }

        public void AddMessageToProtocol(string mid)
        {
            MessageRepository.AddMessageToProtocol(Guid.Parse(mid), Model);
        }


        public void RemoveMessageFromProtocol(string mid)
        {
            MessageRepository.RemoveMessageFromProtocol(Guid.Parse(mid), Model);
        }


        public PartialViewResult AddAllMessagesToProtocol()
        {
            Guid chatID = CurrentUser.Chat.ID;

            MessageRepository.RemoveAllMessagesFromProtocol(chatID, Model);

            var messages = MessageRepository.GetAllMessages(chatID, Model);
            var messageViews = messages.Select(m => new MessageProtocolView(m)).ToList();

            return PartialView("Protocol", messageViews);
        }


        public PartialViewResult RemoveAllMessagesFromProtocol()
        {
            Guid chatID = CurrentUser.Chat.ID;

            MessageRepository.RemoveAllMessagesFromProtocol(chatID, Model);

            var messages = MessageRepository.GetAllMessages(chatID, Model);
            var messageViews = messages.Select(m => new MessageProtocolView(m)).ToList();

            return PartialView("Protocol", messageViews);
        }

        public PartialViewResult ShowAddedMessagesInProtocolPart()
        {
            Guid chatID = CurrentUser.Chat.ID;

            var messages = MessageRepository.GetInProtocolMessages(chatID, Model);
            var messageViews = messages.Select(m => new MessageProtocolView(m)).ToList();

            return PartialView("Protocol", messageViews);
        }
    }
}