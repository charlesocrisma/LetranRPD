using Microsoft.EntityFrameworkCore;

namespace LetranRPD.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options ) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<ServiceInformation> ServiceInformations { get; set; } = null!;
        public DbSet<ServiceProgress> ServiceProgresses { get; set; } = null!;
    }
}
    