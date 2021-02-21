using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyTwitter.Controllers
{
    public class ChatHubController : Controller
    {
        // GET: ChatHub
        public ActionResult Index()
        {
            return View();
        }
    }
}