using Microsoft.EntityFrameworkCore;
using UsersMembers.Domain.Entities.Audit;

namespace UsersMembers.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    }
}
