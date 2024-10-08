﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KnowIT.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
        public DateTime DateCreated {  get; set; }
        public int CategoryID { get; set; }
        public Category category { get; set; }
    }
}
