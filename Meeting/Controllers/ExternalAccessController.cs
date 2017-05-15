using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Meeting.Controllers
{
    public class ExternalAccessController : Controller
    {
        // GET: ExternalAccess
        public ActionResult GetProtocol()
        {
            string chatID = Request.Params["Lnk"];
            string path = Server.MapPath("~/Content/" + chatID + "/");
            path += "protocol.pdf";
            var name = "protocol.pdf";
            var type = "application/octet-stream";

            return File(path, type, name);
        }
    }
}