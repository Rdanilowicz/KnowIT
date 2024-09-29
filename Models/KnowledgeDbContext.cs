using Microsoft.EntityFrameworkCore;

namespace KnowIT.Models
{
    public class KnowledgeDbContext : DbContext
    {
        public KnowledgeDbContext(DbContextOptions<KnowledgeDbContext> options) : base(options)
        {

        }

        public DbSet<KnowledgeItem> KnowledgeItems { get; set; }
    }
}
