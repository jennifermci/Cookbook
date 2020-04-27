using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class AddWrapper
    {
        public RegisterUser RegisterUser {get;set;}
        public Recipe Recipe {get;set;}
    }
}