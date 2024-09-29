using System.ComponentModel;

namespace KnowIT.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated {  get; set; }
        public int CategoryID { get; set; }
        public Category category { get; set; }
    }
}
