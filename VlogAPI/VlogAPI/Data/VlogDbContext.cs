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
        private readonly IConfiguration _configuration;

        public VlogDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetValue<string>("PostgreSQLConnectionString"));
        }
    }
}
