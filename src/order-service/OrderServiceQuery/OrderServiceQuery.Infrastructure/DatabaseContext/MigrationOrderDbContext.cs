

using Microsoft.EntityFrameworkCore;
using OrderServiceQuery.Core.Domain;
using OrderServiceQuery.Core.Repositories;


namespace OrderServiceQuery.Infrastructure.DatabaseContext
{
    public class MigrationOrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public MigrationOrderDbContext(DbContextOptions<MigrationOrderDbContext> options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Order>().ToTable("Order");
        }
    }
}
