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
    public class HomeController : Controller
    {
        BaseRepository<Tweet> _repo = new BaseRepository<Tweet>();

        public ActionResult Index()
        {
            var _user = SessionSet<User>.Get("login");

            List<TweetDto> list = new List<TweetDto>();
            list.Clear();
            if (_user != null)
            {
                var mymodel = (from uf in _repo.Query<UserFriends>()
                               join
                               t in _repo.Query<Tweet>() on uf.FriendId equals t.UserId
                               where (uf.UserId == _user.Id &&
                               t.UserId == uf.FriendId) && t.IsDeleted == false
                               //orderby t.CreateDate descending
                               select new TweetDto()
                               {
                                   Id = t.Id,
                                   Body = t.Body,
                                   UserName = uf.Friend.UserName

                               }).Distinct().ToList();
                mymodel.Reverse();
                foreach (TweetDto model in mymodel)
                {
                    list.Add(model);
                }

                list = mymodel.OrderByDescending(x => x.CreateDate).ToList();
                return View(list);
            }

            else { return RedirectToAction("Login", "Login"); }

        }


        [HttpPost]
        public ActionResult ListTweet(Tweet newTweet)
        {
            var tweetLists = _repo.Query<Tweet>().Reverse().ToList();

            return View(tweetLists);

        }

        public ActionResult AddComment(string text, int tweetId)
        {
            using (BaseRepository<Comment> _repo = new BaseRepository<Comment>())
            {
                var _user = SessionSet<User>.Get("login");
                Comment cmt = new Comment
                {
                    Body = text,
                    UserId = _user.Id,

                    ParentId = tweetId
                };
                _repo.Add(cmt);
                return Json(new { result = true, JsonRequestBehavior.AllowGet });
            }
        }

        public JsonResult GetComment(int id)
        {
            using (BaseRepository<Comment> _repo = new BaseRepository<Comment>())
            {

                var json = (from co in _repo.Query<Comment>()
                            join u in _repo.Query<User>() on co.UserId equals u.Id
                            where co.ParentId == id
                            select new
                            {
                                co.Body,
                                co.CreateDate,
                                co.Id,
                                co.UserId,
                                u.UserName,
                                co.ParentId

                            }
                    ).ToList();

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ReTweet(int tweetId)
        {
            using (BaseRepository<Tweet> _repo = new BaseRepository<Tweet>())
            {
                var _user = SessionSet<User>.Get("login");
                var twId = _repo.Query<Tweet>().FirstOrDefault(x => x.Id == tweetId);
                Tweet tweet = new Tweet
                {
                    Body = twId.Body,
                    UserId = _user.Id,

                    ParentId = tweetId
                };
                _repo.Add(tweet);
                return Json(new { result = true, JsonRequestBehavior.AllowGet });
            }
        }

        public JsonResult GetTweet(int id)
        {
            using (BaseRepository<Tweet> _repo = new BaseRepository<Tweet>())
            {

                var json = (from co in _repo.Query<Tweet>()
                            join u in _repo.Query<User>() on co.UserId equals u.Id
                            where co.Id == id
                            select new
                            {
                                co.Body,
                                co.CreateDate,
                                co.Id,
                                co.UserId,
                                u.UserName,
                                co.ParentId

                            }
                    ).ToList();

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AddLike(int tweetId)
        {
            using (BaseRepository<Like> _repo = new BaseRepository<Like>())
            {
                var _user = SessionSet<User>.Get("login");
                var twId = _repo.Query<Tweet>().FirstOrDefault(x => x.Id == tweetId);
                Like like = new Like
                {
                    Body = twId.Body,
                    UserId = _user.Id,

                    ParentId = tweetId
                };
                _repo.Add(like);
                return Json(new { result = true, JsonRequestBehavior.AllowGet });
            }
        }

        public JsonResult GetLike(int id)
        {
            using (BaseRepository<Like> _repo = new BaseRepository<Like>())
            {

                var json = (from lk in _repo.Query<Like>()
                            join u in _repo.Query<User>() on lk.UserId equals u.Id
                            where lk.Id == id
                            select new
                            {
                                lk.Body,
                                lk.CreateDate,
                                lk.Id,
                                lk.UserId,
                                u.UserName,
                                lk.ParentId

                            }
                    ).ToList();

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }



    }
}