using Microsoft.EntityFrameworkCore;
using WordDaze.Shared;

namespace Database
{
    public class BlogContext : DbContext
    {
        public DbSet<BlogPost> Posts { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {

        }
    }
}
