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
        public DbSet<Journal> Journals { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: set up relationship
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Journal)
                .WithMany(j => j.Articles)
                .HasForeignKey(a => a.JournalId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
    