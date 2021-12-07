namespace DataAccess.Entities
{
    public class UsersInChats
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public UserRoleInChatType Role { get; set; }
    }
}
