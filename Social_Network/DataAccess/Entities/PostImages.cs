using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class PostImages
    {
        public int Id { get; set; }
        public string ImageData { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
