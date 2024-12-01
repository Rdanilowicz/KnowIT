using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KnowIT.Models
{
    public class KnowledgeDbContext : IdentityDbContext<IdentityUser>
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

            // Configure Category-Article relationship without cascade delete
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CategoryID)
                .OnDelete(DeleteBehavior.SetNull); // Set CategoryID to null if the Category is deleted
        }
    }

}
