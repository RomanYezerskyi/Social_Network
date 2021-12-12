using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Interfaces;

namespace DataAccess
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private BaseRepository<Chat> _chats;
        private BaseRepository<Message> _messages;
        private BaseRepository<UsersInChats> _usersInChats;
        private BaseRepository<User> _user;
        private BaseRepository<Friendship> _friendships;
        private BaseRepository<Post> _posts;
        private BaseRepository<PostImages> _images;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IAsyncRepository<Chat> Chats
        { 
            get
            {
                return _chats ??= new BaseRepository<Chat>(_context);
            }
        }

        public IAsyncRepository<Message> Messages
        {
            get
            {
                return _messages ??= new BaseRepository<Message>(_context);
            }
        }
         
        public IAsyncRepository<UsersInChats> UsersInChats
        {
            get
            {
                return _usersInChats ??= new BaseRepository<UsersInChats>(_context);
            }
        }

        public IAsyncRepository<User> User
        {
            get
            {
                return _user ??= new BaseRepository<User>(_context);
            }
        }

        public IAsyncRepository<Friendship> Friendships
        {
            get
            {
                return _friendships ??= new BaseRepository<Friendship>(_context);
            }
        }

        public IAsyncRepository<Post> Posts
        {
            get
            {
                return _posts ??= new BaseRepository<Post>(_context);
            }
        }
        public IAsyncRepository<PostImages> Images
        {
            get
            {
                return _images ??= new BaseRepository<PostImages>(_context);
            }
        }
    }
}
