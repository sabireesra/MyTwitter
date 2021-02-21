using MyTwitter.Models;
using MyTwitter.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyTwitter.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Updates()
        {
            return View();
        }
        public ActionResult GetMessages()
        {
            BaseRepository<Tweet> _messageRepository = new BaseRepository<Tweet>();
            return PartialView("_TweetList", _messageRepository.GetAllMessages());
        }


    }
}