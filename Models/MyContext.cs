using Microsoft.EntityFrameworkCore;
 
namespace Cookbook.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }

        public DbSet<RegisterUser> Users {get;set;}
        public DbSet<Tip> Tips {get;set;}
        public DbSet<Step> Step {get;set;}
        public DbSet<Comment> Comments {get;set;}
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Favorite> Favorites {get;set;}
        public DbSet<Hide> Hides {get;set;}

    }
}