using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class Step
    {
        [Key]
        public int StepId {get; set;}
        [Required]
        public string Text {get;set;}
        public int RecipeId {get;set;}
        public Recipe Recipe {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } =  DateTime.Now;

    }
}