using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class Tip
    {
        [Key]
        public int TipId {get; set;}
        [Required]
        public string Text {get;set;}

        public int UserId {get;set;}
        public RegisterUser User {get;set;}
        public int RecipeId {get;set;}
        public Recipe Recipe {get;set;}
        public List<Comment> Comments {get;set;}
        public List<Like> Likes {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } =  DateTime.Now;

    }
}