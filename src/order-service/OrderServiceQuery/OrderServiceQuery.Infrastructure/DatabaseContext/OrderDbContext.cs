

using Microsoft.EntityFrameworkCore;
using OrderServiceQuery.Core.Domain;
using OrderServiceQuery.Core.Repositories;


namespace OrderServiceQuery.Infrastructure.DatabaseContext
{
    public class OrderDbContext<DbSide> : AppDbContext<DbSide> where DbSide : Side
    {
        public DbSet<Order> Orders { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext<DbSide>> options)
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
