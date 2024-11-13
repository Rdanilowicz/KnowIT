using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowIT.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content is too long")]
        public string Content { get; set; }
        public DateTime DateCreated {  get; set; } =DateTime.Now;
        [ForeignKey("Category")] // Explicit foreign key to Category
        public int? CategoryID { get; set; }
        public Category? Category { get; set; }
    }
}
