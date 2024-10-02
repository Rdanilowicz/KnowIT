using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KnowIT.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime DateCreated {  get; set; }
        public int CategoryID { get; set; }
        public Category category { get; set; }
    }
}
