using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stripe.OnBoardCheckOutSplit.Models;

namespace Stripe.OnBoardCheckOutSplit.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractUser> ContractUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<ApplicationUser>()
                .Property(c => c.StripeAccountStatus)
                .HasConversion<int>();

            modelBuilder.Entity<Project>()
                .Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Milestone>()
                .Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Contract>()
                .Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<ContractUser>()
                .Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");

            base.OnModelCreating(modelBuilder);
        }
    }
}