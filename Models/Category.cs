using System.ComponentModel.DataAnnotations;

namespace KnowIT.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}
