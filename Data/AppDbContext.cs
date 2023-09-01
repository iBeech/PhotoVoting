using Microsoft.EntityFrameworkCore;
using PhotoVotingApp.Models;

namespace PhotovotingApp.Models
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database/photovoting.db");
        }

        public DbSet<Photo> Photos { get; set; }
    }
}