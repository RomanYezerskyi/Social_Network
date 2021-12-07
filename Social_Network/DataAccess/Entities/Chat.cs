using System.Collections;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    
    public class Chat
    {
        public Chat()
        {
            Messages = new List<Message>();
            Users = new List<UsersInChats>();
        }
        public int Id { get; set; }
        public string ChatName { get; set; }
        public string ChatImage { get; set; }
        public string AboutChat { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<UsersInChats> Users { get; set; }
        public ChatType Type { get; set; }

    }
}
