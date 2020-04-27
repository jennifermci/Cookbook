using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId {get; set;}
        [Required]

        public int UserId {get;set;}
        public RegisterUser User {get;set;}
        public int RecipeId {get;set;}
        public Recipe Recipe {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } =  DateTime.Now;

    }
}