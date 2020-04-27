using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class EditWrapper
    {
        public RegisterUser RegisterUser {get;set;}
        public Recipe Recipe {get;set;}

        public Ingredient Ingredient {get;set;}
        public Step Step {get;set;}
    }
}