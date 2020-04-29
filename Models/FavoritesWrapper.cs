using System;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class FavoritesWrapper
    {
        public List<Recipe> RecipeList { get; set; }

        public RegisterUser User { get; set; }

    }

}