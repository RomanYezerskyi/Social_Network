using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Friendship
    {
        public string UserMainId { get; set; }
        public User UserMain { get; set; }
        public string FriendId { get; set; }
        public User Friend { get; set; }
        public FriendshipStatus Status { get; set; }

    }
}
