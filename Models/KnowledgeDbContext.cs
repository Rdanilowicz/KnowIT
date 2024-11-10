using Microsoft.EntityFrameworkCore;

namespace KnowIT.Models
{
    public class KnowledgeDbContext : DbContext
    {
        public KnowledgeDbContext(DbContextOptions<KnowledgeDbContext> options) : base(options)
        {

        }

        public DbSet<Article> Articles { get; set; }
		public DbSet<Category> Categories { get; set; 

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // You can explicitly define the relationship if needed
            modelBuilder.Entity<Article>()
        .HasOne<Category>()  // Specify the Category entity, not the navigation property
        .WithMany()           // No navigation property on Category, so we don't need to specify the collection
        .HasForeignKey(a => a.CategoryID)
        .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
