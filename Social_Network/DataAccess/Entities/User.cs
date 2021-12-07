using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class User : IdentityUser
    {
        public string ProfileImage { get; set; }
        public DateTime MemberSince { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastTimeOnline { get; set; }

        public ICollection<UsersInChats> Chats { get; set; }
        public ICollection<Friendship> Friends { get; set; }
        public ICollection<Friendship> FriendsIAdded { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
