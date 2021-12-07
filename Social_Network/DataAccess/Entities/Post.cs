using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Post
    {
        public int Id { get; set; }
        //public string Title { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public ICollection<PostImages> Images { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
