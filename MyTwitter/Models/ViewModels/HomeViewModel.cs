using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTwitter.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
    }

}