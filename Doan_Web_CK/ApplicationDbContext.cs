using Doan_Web_CK.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Doan_Web_CK
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Nofitication> Nofitications { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(u => u.Blogs)
            //    .WithOne(b => b.Account)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(u => u.Nofitications)
            //    .WithOne(n => n.RecieveAccount)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(u => u.Friendships)
            //    .WithOne(f => f.User)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Like>()
            //    .HasOne(l => l.ApplicationUser)
            //    .WithMany()
            //    .HasForeignKey(l => l.ApplicationUserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Comment>()
            //    .HasOne(c => c.Account)
            //    .WithMany()
            //    .HasForeignKey(c => c.AccountId)
            //    .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Nofitication>()
               .HasOne(n => n.Blog)
               .WithMany()
               .HasForeignKey(n => n.BlogId)
               .OnDelete(DeleteBehavior.Cascade);

            // friendShip
            modelBuilder.Entity<Friendship>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friendships)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);


            // nofitication
            modelBuilder.Entity<Nofitication>()
               .HasOne(n => n.RecieveAccount)
               .WithMany(a => a.Nofitications)
               .HasForeignKey(n => n.RecieveAccountId)
               .IsRequired(false);

            modelBuilder.Entity<Nofitication>()
                .HasOne(n => n.SenderAccount)
                .WithMany()
                .HasForeignKey(n => n.SenderAccountId)
                .IsRequired(false);

            // Các cài đặt khác của modelBuilder
            base.OnModelCreating(modelBuilder);
        }
    }
}
