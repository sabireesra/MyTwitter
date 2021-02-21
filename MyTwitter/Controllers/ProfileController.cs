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
    public class ProfileController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        BaseRepository<User> uRepo = new BaseRepository<User>();
        BaseRepository<Like> _repo = new BaseRepository<Like>();

        // GET: Profile
        public ActionResult Index()
        {
            var _user = SessionSet<User>.Get("login");
            if (_user != null)
            {
                using (BaseRepository<Tweet> _repo = new BaseRepository<Tweet>())
                {
                    var mymodel = (from u in _repo.Query<User>()
                                   join
                                   t in _repo.Query<Tweet>() on u.Id equals t.UserId
                                   where (u.Id == _user.Id) && t.IsDeleted == false
                                   orderby t.CreateDate descending
                                   select new TweetDto()
                                   {
                                       Id = t.Id,
                                       Body = t.Body,
                                       UserName = u.UserName,
                                   }).Distinct().ToList();
                    mymodel.Reverse();
                    return View(mymodel);
                }
            }
            else { return RedirectToAction("Login", "Login"); }
        }

        public ActionResult FollowerList()
        {
            var _user = SessionSet<User>.Get("login");
            int myId = _user.Id;

            var friends = db.Database.SqlQuery<string>(
                                  "SELECT UserName FROM Users where Id IN(SELECT UserId FROM UserFriends where FriendId =" + myId + ")").ToList();
            List<User> list = new List<User>();
            foreach (string item in friends)
            {
                var model = db.Users.FirstOrDefault(x => x.UserName == item);
                list.Add(model);
            }
            return View(list);
        }

        public ActionResult FollowingList()
        {
            List<UserFriends> newList = new List<UserFriends>();
            var _user = SessionSet<User>.Get("login");
            int myId = _user.Id;
            var myFollowing = db.UserFriends.Where(k => k.UserId == myId).ToList();
            foreach (var item in myFollowing)
            {
                var user = db.Users.Where(k => k.Id == item.FriendId).First();
                UserFriends yeni = new UserFriends()
                {
                    UserId = item.UserId,
                    FriendId = item.FriendId,
                    User = db.Users.Where(k => k.Id == user.Id).First()
                };
                newList.Add(yeni);
            }
            return View(newList);
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

            var result = uRepo.Query<UserFriends>().Where(k => (k.UserId == uf.UserId) && (k.FriendId == uf.FriendId)).FirstOrDefault();
            if (result == null)
            {
                db.UserFriends.Add(uf);
                db.SaveChanges();
            }
            return RedirectToAction("FollowingList", "Profile");

            // return View();


        }
        [HttpGet]
        public ActionResult TweetId(int id)
        {
            Comment com = new Comment();
            com.Id = id;
            SessionSet<Comment>.Set(com, "get");
            return View();
        }


        [HttpPost]
        public ActionResult Liked(Like likeTweet)
        {
            var id = SessionSet<Comment>.Get("get").Id;
            var friend = uRepo.Query<Tweet>().FirstOrDefault(k => k.Id == id);

            likeTweet.UserId = SessionSet<User>.Get("login").Id;
            likeTweet.ParentId = friend.Id;

            _repo.Add(likeTweet);
            return RedirectToAction("Index", "Home");

        }

        //public ActionResult LikedList()
        //{

        //    List<Tweet> newList = new List<Tweet>();
        //    var _user = SessionSet<User>.Get("login");
        //    int myId = _user.Id;
        //    var myLikes = db.Tweets.Where(k => k.ParentId == myId).ToList();
        //    foreach (var item in myLikes)
        //    {
        //        var likedtweet = db.Like.Where(k => k.UserId == item.FriendId).First();
        //        UserFriends yeni = new UserFriends()
        //        {
        //            UserId = item.UserId,
        //            FriendId = item.,
        //            User = db.Users.Where(k => k.Id == user.Id).First()
        //        };
        //        newList.Add(yeni);
        //    }
        //    return View(newList);
        //}
    }
}