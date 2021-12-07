using DataAccess.Entities;

namespace DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        IAsyncRepository<Chat> Chats { get; }
        IAsyncRepository<Message> Messages { get; }
        IAsyncRepository<UsersInChats> UsersInChats { get; }
        IAsyncRepository<User> User { get; }
        IAsyncRepository<Friendship> Friendships {get;}
        IAsyncRepository<Post> Posts { get; }
        IAsyncRepository<PostImages> Images { get; }
    }
}
