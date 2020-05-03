using AniMediaNotifier.Core;
using Microsoft.EntityFrameworkCore;

namespace AniMediaNotifier.Data
{
	public class AniMediaNotifierDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Title> Titles { get; set; }
		public DbSet<UserTitle> UserTitles { get; set; }

		public AniMediaNotifierDbContext(DbContextOptions<AniMediaNotifierDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasKey(u => u.Id);

			modelBuilder.Entity<User>()
				.Property(u => u.Id).ValueGeneratedOnAdd();

			modelBuilder.Entity<Title>()
				.HasKey(t => t.Id);

			modelBuilder.Entity<Title>()
				.Property(t => t.Id).ValueGeneratedOnAdd();

			modelBuilder.Entity<UserTitle>()
				.HasKey(ut => new { ut.UserId, ut.TitleId });

			modelBuilder.Entity<UserTitle>()
				.HasOne(ut => ut.User)
				.WithMany(u => u.UserTitles)
				.HasForeignKey(ut => ut.UserId);

			modelBuilder.Entity<UserTitle>()
				.HasOne(ut => ut.Title)
				.WithMany(t => t.UserTitles)
				.HasForeignKey(ut => ut.TitleId);
		}
	}
}