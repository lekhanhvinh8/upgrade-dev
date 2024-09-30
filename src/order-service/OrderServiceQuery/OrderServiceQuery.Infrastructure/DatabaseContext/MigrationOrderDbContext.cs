

using Microsoft.EntityFrameworkCore;
using OrderServiceQuery.Core.Domain;
using OrderServiceQuery.Core.Repositories;


namespace OrderServiceQuery.Infrastructure.DatabaseContext
{
    public class MigrationOrderDbContext : OrderDbContext<ReadSide>
    {
        public MigrationOrderDbContext(DbContextOptions<OrderDbContext<ReadSide>> options)
            :base(options)
        {
        }
       
    }
}
