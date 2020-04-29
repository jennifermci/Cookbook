using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage="Recipe name must have at least 2 characters")]
        public string RecipeName { get; set; }

        [Required]
        public int RecipeDuration { get; set; }

        public string RecipeImage { get; set; }

        public int CreatorId {get;set;}
        public RegisterUser Creator {get;set;}

        public List<Favorite> UserFavorites {get;set;}

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<Ingredient> IngredientList { get; set; }
        public List<Step> StepList { get; set; }
        public List<Tip> Tips {get;set;}
        public List<Hide> Hides {get;set;}
    }
}