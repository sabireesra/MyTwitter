using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTwitter.Models
{
    public class TweetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ParentId { get; set; }
        public string UserName { get; set; }
        public string Body { get; set; }
        public DateTime CreateDate { get; set; }
        public User User { get; set; }
        public int? ParentTweetId { get; set; }
        public Tweet ParentTweet { get; set; }
        public bool IsDeleted { get; set; }
    }
}