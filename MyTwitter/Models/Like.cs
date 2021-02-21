using MyTwitter.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTwitter.Models
{
    public class Like : BaseModel
    {
        public User User { get; set; }
        public int UserId { get; set; }
        public string Body { get; set; }

        public Tweet ParentTweet { get; set; }
        public int ParentId { get; set; }

    }
}