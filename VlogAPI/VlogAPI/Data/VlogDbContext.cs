using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VlogAPI.Auth.Model;
using VlogAPI.Data.Entities;
using System.Reflection.Emit;

namespace VlogAPI.Data
{
    public class VlogDbContext : IdentityDbContext<VlogUser>
    {
        public DbSet<Post>? Posts { get; set; }
        public DbSet<Comment>? Comments { get; set; }
        public DbSet<Like>? Likes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=VlogDb");
        }
    }
}
