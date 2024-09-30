
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderServiceQuery.Core.Repositories;


namespace OrderServiceQuery.Infrastructure.DatabaseContext
{
    public class AppDbContext<DbSide> : DbContext where DbSide : Side
    {
        private readonly DbContextOptions _options;

        public AppDbContext(DbContextOptions options)
            :base(options)
        {
            this._options = options;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Disable default logging.
            //optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddFilter((category, level) => false)));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, bool autoSaveHistory = true, bool autoSaveCreatedDate = true, bool autoSaveUpdatedDate = true)
        {
            try
            {
                if(typeof(DbSide) == typeof(ReadSide))
                {
                    return -1;
                }

                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
               
            }
            
            return -1;
        }
    }
}