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
    public class LoginController : Controller
    {
        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }


        //[HttpPost]

        //public ActionResult Login(User model)
        //{


        //    using(BaseRepository<User> loginRepo = new BaseRepository<User>())
        //    {

        //        var user = loginRepo.Query<User>().Where(k => k.Email == model.Email 
        //        && k.Password == model.Password).ToList();
        //        if(user != null)
        //        {
        //            User _temp = new User();
        //            foreach (var item in user)
        //            {
        //                _temp.UserName = item.UserName;
        //                _temp.Email = item.Email;
        //                _temp.Id = item.Id;
        //            }

        //            SessionSet<User>.Set(_temp, "login");
        //            return RedirectToAction("Index","Home");

        //        }
        //        else
        //        {
        //            return View();
        //        }

        //    }     

        //}


        [HttpPost]
        public ActionResult Login(User model)
        {
            BaseRepository<User> _repo = new BaseRepository<User>();

            var repo = _repo.Query<User>().Where(x => x.Email == model.Email && x.Password == model.Password).ToList();

            if (repo.Count > 0)
            {
                User _temp = new User();
                foreach (var item in repo)
                {
                    _temp.UserName = item.UserName;
                    _temp.Email = item.Email;
                    _temp.Id = item.Id;
                    //_temp.LastName = item.LastName;
                    //_temp.Dis = item.Dis;

                }

                SessionSet<User>.Set(_temp, "login");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Hatalı Giriş Yaptınız");
                return View();
            }

        }
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Register(User model)
        {


            using (BaseRepository<User> loginRepo = new BaseRepository<User>())
            {

                loginRepo.Add(model);
                return RedirectToAction("Login");

            }



        }

        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


    }
}