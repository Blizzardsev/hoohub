using Microsoft.EntityFrameworkCore;

namespace hoohub.Data
{
    public class HooHubContext : DbContext
	{
		private readonly string _sqLitePath = "context.db";
		public DbSet<Comic> Comics { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<HooHubUser> Users { get; set; }

		public HooHubContext()
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Data Source={_sqLitePath}");
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Comic>().ToTable("Comics").HasKey("Guid");
			modelBuilder.Entity<Event>().ToTable("Events").HasKey("Guid");
            modelBuilder.Entity<HooHubUser>().ToTable("Users").HasKey("Guid");
        }
	}
}
