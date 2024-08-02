using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DeckDJ.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class CategoryDto
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}