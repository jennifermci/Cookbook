using System;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class DetailsWrapper
    {
        public Recipe Recipe { get; set; }
        public List<Ingredient> IngredientsList { get; set; }
        public List<Step> StepsList { get; set; }

        public RegisterUser User { get; set; }
    }

}