using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace hoohub.Data
{
    public class HooHubContext : IdentityDbContext<HooHubUser>
    {
		public DbSet<Comic> Comics { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<HooHubUser> Users { get; set; }
		public DbSet<ComicLike> ComicLikes { get; set; }

		public HooHubContext()
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DB_STRING"));
            base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new HooHubUserEntityConfiguration());

            modelBuilder.Entity<Comic>().ToTable("Comics").HasKey("Id");
			modelBuilder.Entity<Event>().ToTable("Events").HasKey("Id");
            modelBuilder.Entity<HooHubUser>().ToTable("Users").HasKey("Id");
            modelBuilder.Entity<ComicLike>().ToTable("ComicLikes").HasKey("Id");
        }

        public class HooHubUserEntityConfiguration : IEntityTypeConfiguration<HooHubUser>
        {
            public void Configure(EntityTypeBuilder<HooHubUser> builder)
            {
            }
        }
    }
}
