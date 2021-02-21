using MyTwitter.Models;
using MyTwitter.Models.Repository;
using MyTwitter.SessionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyTwitter.Controllers
{
    public class TweetController : Controller
    {

        DatabaseContext db = new DatabaseContext();
        BaseRepository<Tweet> loginRepo = new BaseRepository<Tweet>();

        // GET: Tweet
        public ActionResult Index()
        {
            return View();

        }
        public ActionResult AddTweet()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddTweet(Tweet newTweet)
        {
            newTweet.UserId = SessionSet<User>.Get("login").Id;
            newTweet.ParentId = 0;

            loginRepo.Add(newTweet);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CommentList(int id)
        {
            var _user = SessionSet<User>.Get("login");
            var anytweet = loginRepo.Query<Tweet>().FirstOrDefault(k => k.Id == id);
            // Tweet tweet = new Tweet();
            List<TweetDto> list = new List<TweetDto>();
            list.Clear();
            if (_user != null)
            {
                var mymodel = (from t in loginRepo.Query<Comment>()

                               where (t.ParentId == anytweet.Id) && t.IsDeleted == false
                               select new TweetDto()
                               {
                                   Id = t.Id,
                                   Body = t.Body,
                                   UserName = t.User.UserName

                               }).ToList();
                mymodel.Reverse();
                foreach (TweetDto model in mymodel)
                {

                    list.Add(model);

                }

                //list = mymodel.OrderByDescending(x => x.CreateDate).ToList();


                return View(list);
            }
            return View();
        }


        public ActionResult Delete()
        {
            return View();
        }



        [HttpGet]
        public ActionResult Delete(int id)
        {
            var tweet = loginRepo.Query<Tweet>().FirstOrDefault(k => k.Id == id);
            if (tweet != null)
            {
                loginRepo.Delete(tweet);

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult Onerilen()
        {
            var _user = SessionSet<User>.Get("login");

            int FriendId = Convert.ToInt32(RouteData.Values["id"]);
            int myId = _user.Id;
            var notfriends = db.Database.SqlQuery<int>(
                                  "SELECT Id FROM Users where Id not IN(SELECT FriendId FROM UserFriends where UserId =   " + myId + ") and ID!=" + myId).ToList();
            List<User> list = new List<User>();
            foreach (int item in notfriends)
            {

                var model = db.Users.FirstOrDefault(x => x.Id == item);

                if (model.IsDeleted == false)
                {
                    list.Add(model);
                }


            }
            return View(list);


        }

        [HttpGet]
        public ActionResult Follow()
        {

            var _user = SessionSet<User>.Get("login");

            int FriendId = Convert.ToInt32(RouteData.Values["id"]);
            int myId = _user.Id;
            UserFriends uf = new UserFriends();
            uf.UserId = myId;
            uf.FriendId = FriendId;
            uf.CreateDate = DateTime.Now;

            var result = loginRepo.Query<UserFriends>().Where(k => (k.UserId == uf.UserId) && (k.FriendId == uf.FriendId)).FirstOrDefault();
            if (result == null)
            {
                db.UserFriends.Add(uf);

                db.SaveChanges();


            }
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public ActionResult UnFollow(int Id)
        {
            var friend = db.UserFriends.FirstOrDefault(k => k.FriendId == Id);
            if (friend != null)
                loginRepo.UnFollow(friend.Id);
            return RedirectToAction("FollowingList", "Profile");
        }

        public ActionResult ReTweet()
        {
            return View();
        }

    }
}