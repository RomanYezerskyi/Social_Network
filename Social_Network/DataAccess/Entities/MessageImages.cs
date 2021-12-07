using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class MessageImages
    {
        public int Id { get; set; }
        public string ImageData { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}
