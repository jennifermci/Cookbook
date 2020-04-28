using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cookbook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cookbook.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult RegisterUser(RegisterUser regFromForm)
        {

            if(ModelState.IsValid)
            {
                PasswordHasher<RegisterUser> Hasher = new PasswordHasher<RegisterUser>();
                regFromForm.Password = Hasher.HashPassword(regFromForm, regFromForm.Password);

                if(dbContext.Users.Any(u => u.Email == regFromForm.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }                
                dbContext.Add(regFromForm);
                dbContext.SaveChanges();
                HttpContext.Session.SetObjectAsJson("LoggedInUser", regFromForm);
                return RedirectToAction("Main");            
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult LoginUser(LoginUser LoginFromForm)
        {
           if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == LoginFromForm.LoginEmail);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LoginEmail", "Invalid Email or Password");
                    return View("Index");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(LoginFromForm, userInDb.Password, LoginFromForm.LoginPassword);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email or Password");
                    return View("Index");
                } 
                else
                {
                    HttpContext.Session.SetObjectAsJson("LoggedInUser", userInDb);
                    return RedirectToAction("Main");
                }
                
            }

            return View("Index");
        }


        [HttpGet("logout")]

        public ViewResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        [HttpGet("main")]
        public IActionResult Main()
        {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            if(fromLogin == null)
            {
                return RedirectToAction("Index");
            }
            MainWrapper MainWrapper = new MainWrapper();
            MainWrapper.RegisterUser = fromLogin;
            MainWrapper.RecipeList = dbContext.Recipes.ToList();

            return View("Main", MainWrapper);
        }

        [HttpGet("newrecipe")]
        public IActionResult AddRecipe()
        {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            if(fromLogin == null)
            {
                return RedirectToAction("Index");
            }
            AddWrapper AddWrapper = new AddWrapper();
            AddWrapper.RegisterUser = fromLogin;
            return View("AddRecipe", AddWrapper);
        }
        [HttpPost("createrecipe")]
        public IActionResult CreateRecipe(AddWrapper fromForm)
        {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            if(ModelState.IsValid)
            {
                dbContext.Add(fromForm.Recipe);
                dbContext.SaveChanges();
                Recipe Recipe = dbContext.Recipes
                    .Include(r => r.IngredientList)
                    .Include(r => r.StepList)
                    .Last();
                EditWrapper EditWrapper = new EditWrapper();
                EditWrapper.RegisterUser = fromLogin;
                EditWrapper.Recipe = Recipe;
                return View("EditRecipe", EditWrapper);
            }
            else
            {
                AddWrapper AddWrapper = new AddWrapper();
                AddWrapper.RegisterUser = fromLogin;
                return View("AddRecipe", AddWrapper);
            }
        }

        [HttpGet("editrecipe/{RecipeId}")]
        public IActionResult EditRecipe(int RecipeId){
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            EditWrapper EditWrapper = new EditWrapper();
            Recipe Recipe = dbContext.Recipes
                .Include(r => r.IngredientList)
                .Include(r => r.StepList)
                .FirstOrDefault(r => r.RecipeId == RecipeId);

            EditWrapper.RegisterUser = fromLogin;
            EditWrapper.Recipe = Recipe;
            return View("EditRecipe", EditWrapper);
        }

        [HttpPost("addingredient")]
        public IActionResult AddIngredient(EditWrapper fromForm)
        {
            if(ModelState.IsValid)
            {
                dbContext.Add(fromForm.Ingredient);
                dbContext.SaveChanges();
                int RecipeId = fromForm.Ingredient.RecipeId;
                RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
                EditWrapper EditWrapper = new EditWrapper();
                Recipe Recipe = dbContext.Recipes
                    .Include(r => r.IngredientList)
                    .Include(r => r.StepList)
                    .FirstOrDefault(r => r.RecipeId == fromForm.Ingredient.RecipeId);
                EditWrapper.RegisterUser = fromLogin;
                EditWrapper.Recipe = Recipe;
                return View("EditRecipe", EditWrapper);
            }
            else
            {
                RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
                EditWrapper EditWrapper = new EditWrapper();
                Recipe Recipe = dbContext.Recipes
                    .Include(r => r.IngredientList)
                    .Include(r => r.StepList)
                    .FirstOrDefault(r => r.RecipeId == fromForm.Ingredient.RecipeId);
                EditWrapper.RegisterUser = fromLogin;
                EditWrapper.Recipe = Recipe;
                return View("EditRecipe", EditWrapper);
            }
        }

        [HttpPost("addstep")]
        public IActionResult AddStep(EditWrapper fromForm)
        {
            if(ModelState.IsValid)
            {
                dbContext.Add(fromForm.Step);
                dbContext.SaveChanges();
                int RecipeId = fromForm.Step.RecipeId;
                RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
                EditWrapper EditWrapper = new EditWrapper();
                Recipe Recipe = dbContext.Recipes
                    .Include(r => r.IngredientList)
                    .Include(r => r.StepList)
                    .FirstOrDefault(r => r.RecipeId == fromForm.Step.RecipeId);
                EditWrapper.RegisterUser = fromLogin;
                EditWrapper.Recipe = Recipe;
                return View("EditRecipe", EditWrapper);
            }
            else
            {
                RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
                EditWrapper EditWrapper = new EditWrapper();
                Recipe Recipe = dbContext.Recipes
                    .Include(r => r.IngredientList)
                    .Include(r => r.StepList)
                    .FirstOrDefault(r => r.RecipeId == fromForm.Step.RecipeId);
                EditWrapper.RegisterUser = fromLogin;
                EditWrapper.Recipe = Recipe;
                return View("EditRecipe", EditWrapper);
            }
        }
        [HttpGet("editUser")]
        public IActionResult EditUser()
        {
            return View();
        }

        [HttpGet("details/{RecipeId}")]
        public IActionResult RecipeDetails(int RecipeId)
        {
            Recipe ThisRecipe = dbContext.Recipes
                .Include(r => r.IngredientList)
                .Include(r => r.StepList)
                .FirstOrDefault(r => r.RecipeId == RecipeId);
            DetailsWrapper DetailsWrapper = new DetailsWrapper();
            DetailsWrapper.Recipe = ThisRecipe;


            return View(DetailsWrapper);
        }

        [HttpGet("savedRecipes")]
        public IActionResult SavedRecipes()
        {
            return View();
        }

    }


    public static class SessionExtensions
    {
        // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            // This helper function simply serializes theobject to JSON and stores it as a string in session
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        
        // generic type T is a stand-in indicating that we need to specify the type on retrieval
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            // Upon retrieval the object is deserialized based on the type we specified
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
