using Microsoft.EntityFrameworkCore;
using ScrapingChallenge.Domain.Models;

namespace ScrapingChallenge.Infrastructure.Context
{
    public class ScrapingDbContext : DbContext
    {
        public virtual DbSet<MenuItem> MenuItems { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Dish> Dishes { get; set; }

        public ScrapingDbContext()
        {
            
        }

        public ScrapingDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
