using Microsoft.EntityFrameworkCore;

using Demo.Entities;

namespace Demo.DAL.Context
{
    public class DemoDbContext : DbContext
    {

        public DemoDbContext()
        {
        }
        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        //Model exemple : public virtual DbSet<ModelName> ModelName { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}

