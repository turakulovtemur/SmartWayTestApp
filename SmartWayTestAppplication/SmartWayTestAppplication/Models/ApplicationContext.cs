using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SmartWayTestAppplication.Models
{
    public class ApplicationContext : IdentityDbContext<User,Role,long>
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<DownloadLink> DownloadLinks { get; set; }
        public DbSet<FileDownloadLink> FilesDownloadLinks { get; set; }     
        public DbSet<Token> Tokens { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
           : base(options)
        {            
           // Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
    }
}
