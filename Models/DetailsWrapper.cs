using System;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class DetailsWrapper
    {
        public Recipe Recipe { get; set; }
        public List<Ingredient> IngredientsList { get; set; }
        public List<Recipe> RecipeList { get; set; }
        public List<Step> StepsList { get; set; }

        public Tip Tip {get;set;}
        public Comment Comment {get;set;}
        public RegisterUser User { get; set; }

        public Like Like {get;set;}
    }

}

