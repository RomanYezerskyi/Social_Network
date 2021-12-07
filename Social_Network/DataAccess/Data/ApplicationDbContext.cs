using DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UsersInChats> UsersInChats { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImages> PostImages { get; set; }
        public DbSet<MessageImages> MessageImages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UsersInChats>().HasKey(x => new { x.ChatId, x.UserId });

            builder.Entity<Friendship>().HasKey(x => new { UserId = x.UserMainId, x.FriendId });
            builder.Entity<Friendship>()
                .HasOne(f => f.UserMain)
                .WithMany(mu => mu.FriendsIAdded)
                .HasForeignKey(f => f.UserMainId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany(mu => mu.Friends)
                .HasForeignKey(f => f.FriendId).OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Message>()
            //    .HasOne(x => x.Chat)
            //    .WithMany(x => x.Messages)
            //    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Post>()
                .HasOne(u => u.User)
                .WithMany(p => p.Posts)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostImages>()
                .HasOne(p => p.Post)
                .WithMany(i => i.Images)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MessageImages>()
                .HasOne(p => p.Message)
                .WithMany(i => i.Images)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}