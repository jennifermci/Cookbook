using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }

        [Required]
        public string  IngredientName { get; set; }

        [Required]
        public decimal  Quantity { get; set; }

        [Required]
        public string  Metric { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}