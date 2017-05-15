using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Meeting.Models;
using Meeting.Models.Repository;
using Meeting.Models.ViewModels;
using Meeting.Helpers;

namespace Meeting.Controllers
{
    public class UsersController : BaseController
    {
        // GET: Users
        public ActionResult Index()
        {
            return View(Model.UserSet.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = Model.UserSet.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        private Chat RequestedChat { get; set; }


        [HttpGet]
        public ActionResult Login()
        {
                var user = Auth.Login(Request.Params["Lnk"]);
            
                if (user != null)
                {
                    RequestedChat = user.Chat;
                    int c = RequestedChat.StartingTime.CompareTo(DateTime.Now);
                    if (c > 0)
                       return ReturnLoginError("Совещание начнется " + RequestedChat.StartingTime.ToShortDateString() + " в " + RequestedChat.StartingTime.ToShortTimeString());
                    //TODO проверка на завершенность совещания
                
                    return RedirectToAction("Index", "MainChat");
                }

            return ReturnLoginError("Отказано в доступе");
        }

        private ActionResult ReturnLoginError(string errorStr)
        {
            ViewBag.LoginError = errorStr;
            return View();
        }

        // GET: Users/Create
        /*        public ActionResult Create()
             {
                 return View();
             }

             // POST: Users/Create
             // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
             // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
              [HttpPost]
                 [ValidateAntiForgeryToken]
                 public ActionResult Create([Bind(Include = "Nick,Password")] User user)
                 {
                     if (ModelState.IsValid)
                     {
                         string pass = user.Password;
                         user.ID = Guid.NewGuid();
                         user.Type = 1000;
                         user.Chat = ChatRepository.GetChatByID(Guid.Parse("00000000-0000-0000-0000-000000000001"), Model);
                         bool isUserAdded = ChatRepository.AddNewUser(user, Model);  //Почему здесь переменная user имеет хешированный пароль после вызова метода???

                         if (isUserAdded)
                         {
                    //         ChatID = "00000000-0000-0000-0000-000000000001";
                      //       var userAuth = Auth.Login(user.Nick, pass, ChatID, false);
                        //     return RedirectToAction("Index", "MainChat");
                         }

                     }

                     return View(user);
                 } */

        // GET: Users/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = Model.UserSet.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Nick,EJIK_ID,Type")] User user)
        {
            if (ModelState.IsValid)
            {
                Model.Entry(user).State = EntityState.Modified;
                Model.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = Model.UserSet.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            User user = Model.UserSet.Find(id);
            Model.UserSet.Remove(user);
            Model.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Model.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
