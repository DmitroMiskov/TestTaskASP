using System.Security.Cryptography;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {

        }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>().HasKey(m => m.Id);
            modelBuilder.Entity<Message>().Property(m => m.Username).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Message>().Property(m => m.Content).IsRequired();
            modelBuilder.Entity<Message>().Property(m => m.Timestamp).IsRequired();
        }
    }
}