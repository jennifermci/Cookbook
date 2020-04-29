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
            RegisterUser User = dbContext.Users
                .Include(u=>u.Hides)
                .FirstOrDefault(u => u.UserId == fromLogin.UserId); 
            MainWrapper MainWrapper = new MainWrapper();
            MainWrapper.RegisterUser = User;
            
            List<Recipe> AllRecipes = dbContext.Recipes.ToList();
            foreach(var hide in User.Hides)
            {
                Recipe temp = dbContext.Recipes.FirstOrDefault(r => r.RecipeId == hide.RecipeId);
                AllRecipes.Remove(temp);
            }
            MainWrapper.RecipeList = AllRecipes;

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

        [HttpGet("editUser/{userId}")]
        public IActionResult EditUser(int userId)
        {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            if(fromLogin == null)
            {
                return RedirectToAction("Index");
            }
            RegisterUser ToEdit = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            return View(ToEdit);
        }

        [HttpPost("update/{userId}")]
        public IActionResult EditThisUser(int userId, RegisterUser fromForm)
        {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");

            RegisterUser User = dbContext.Users.FirstOrDefault(r => r.UserId == fromLogin.UserId); 
            User.FirstName = fromForm.FirstName;
            User.LastName = fromForm.LastName;
            User.UserId = userId;
            User.Email = fromForm.Email;

            // if(ModelState.IsValid)
            // {

                if(dbContext.Users.Any(u => u.Email == fromForm.Email ))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("EditUser", User);
                } 
                dbContext.Update(User);
                dbContext.Entry(User).Property("CreatedAt").IsModified = false;
                dbContext.Entry(User).Property("Password").IsModified = false;
                dbContext.SaveChanges();

                HttpContext.Session.SetObjectAsJson("LoggedInUser", User);

                return RedirectToAction("Main");
            // }
            // else
            // {
            //     return View("EditUser", fromForm);
            // }

        }

        [HttpGet("details/{RecipeId}")]
        public IActionResult RecipeDetails(int RecipeId)
        {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            RegisterUser RegisterUser = dbContext.Users
                .Include(u => u.TipLikes)
                .FirstOrDefault(u=>u.UserId == fromLogin.UserId);
            Recipe ThisRecipe = Enumerable.Reverse(dbContext.Recipes
                .Include(r => r.IngredientList)
                .Include(r => r.StepList)
                .Include(r=>r.Tips)
                .ThenInclude(t => t.User)
                .Include(r=>r.Tips)
                .ThenInclude(t=> t.Comments)
                .Include(r=>r.Tips)
                .ThenInclude(t=> t.Likes))
                .FirstOrDefault(r => r.RecipeId == RecipeId);
            DetailsWrapper DetailsWrapper = new DetailsWrapper();
            DetailsWrapper.Recipe = ThisRecipe;
            DetailsWrapper.User = RegisterUser;


            return View(DetailsWrapper);
        }
        [HttpPost("addtip")]
        public IActionResult AddTip(DetailsWrapper fromForm)
        {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            if(ModelState.IsValid)
            {
                dbContext.Add(fromForm.Tip);
                dbContext.SaveChanges();
                var RecipeId = fromForm.Tip.RecipeId;
                return Redirect($"details/{RecipeId}");
            }
            else 
            {
                Recipe ThisRecipe = Enumerable.Reverse(dbContext.Recipes
                    .Include(r => r.IngredientList)
                    .Include(r => r.StepList)
                    .Include(r=>r.Tips)
                    .ThenInclude(t => t.User)
                    .Include(r=>r.Tips)
                    .ThenInclude(t=> t.Comments)
                    .Include(r=>r.Tips)
                    .ThenInclude(t=> t.Likes))
                    .FirstOrDefault(r => r.RecipeId == fromForm.Tip.RecipeId);
                DetailsWrapper DetailsWrapper = new DetailsWrapper();
                DetailsWrapper.Recipe = ThisRecipe;
                DetailsWrapper.User = fromLogin;
            return View("RecipeDetails",DetailsWrapper);
            }
        }

        // [HttpGet("savedRecipes")]
        // public IActionResult SavedRecipes(int UserId)
        // {
        //     RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
        //     if(fromLogin == null)
        //     {
        //         return RedirectToAction("Index");
        //     }

        //     FavoritesWrapper FavoritesWrapper = new FavoritesWrapper();
        //     RecipeList recipeList = dbContext.Recipes
        //         .Include(f => f.UserFavorites).ToList();

        //     FavoritesWrapper.RecipeList = RecipeList;
        //     return View(FavoritesWrapper);
        // }

        [HttpPost("addcomment")]
        public IActionResult AddComment(DetailsWrapper fromForm, int RecipeId)
        {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            if(ModelState.IsValid)
            {
                dbContext.Add(fromForm.Comment);
                dbContext.SaveChanges();
                return Redirect($"details/{RecipeId}");
            }
            else 
            {
                Recipe ThisRecipe = Enumerable.Reverse(dbContext.Recipes
                    .Include(r => r.IngredientList)
                    .Include(r => r.StepList)
                    .Include(r=>r.Tips)
                    .ThenInclude(t => t.User)
                    .Include(r=>r.Tips)
                    .ThenInclude(t=> t.Comments)
                    .Include(r=>r.Tips)
                    .ThenInclude(t=> t.Likes))
                    .FirstOrDefault(r => r.RecipeId == RecipeId);
                DetailsWrapper DetailsWrapper = new DetailsWrapper();
                DetailsWrapper.Recipe = ThisRecipe;
                DetailsWrapper.User = fromLogin;

                return View("RecipeDetails",DetailsWrapper);
            }
        }
        [HttpPost("addtiplike")]
        public IActionResult AddTipLike(DetailsWrapper fromForm, int RecipeId)
        {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            if(ModelState.IsValid)
            {
                dbContext.Add(fromForm.Like);
                dbContext.SaveChanges();
                return Redirect($"details/{RecipeId}");
            }
            else
            {
                Recipe ThisRecipe = Enumerable.Reverse(dbContext.Recipes
                    .Include(r => r.IngredientList)
                    .Include(r => r.StepList)
                    .Include(r=>r.Tips)
                    .ThenInclude(t => t.User)
                    .Include(r=>r.Tips)
                    .ThenInclude(t=> t.Comments)
                    .Include(r=>r.Tips)
                    .ThenInclude(t=> t.Likes))
                    .FirstOrDefault(r => r.RecipeId == RecipeId);
                DetailsWrapper DetailsWrapper = new DetailsWrapper();
                DetailsWrapper.Recipe = ThisRecipe;
                DetailsWrapper.User = fromLogin;

                return View("RecipeDetails",DetailsWrapper);
            }
        }
        [HttpPost("deletetiplike")]
        public IActionResult RemoveTipLike(DetailsWrapper fromForm, int RecipeId)
        {
            Like ToDelete = dbContext.Likes.FirstOrDefault(l => l.UserTipId == fromForm.Like.UserTipId && l.TipId == fromForm.Like.TipId);
            dbContext.Remove(ToDelete);
            dbContext.SaveChanges();
            return Redirect($"details/{RecipeId}");
        }

       [HttpGet("addhide/{RecipeId}")] 
       public IActionResult AddHide(int RecipeId)
       {
           RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
           Hide Hide = new Hide();
           Hide.RecipeId = RecipeId;
           Hide.UserId = fromLogin.UserId;
           dbContext.Add(Hide);
           dbContext.SaveChanges();
           return RedirectToAction("Main");
       }

       [HttpGet("addfavorite/{RecipeId}")]
       public IActionResult AddFavorite(int RecipeId)
       {
            RegisterUser fromLogin = HttpContext.Session.GetObjectFromJson<RegisterUser>("LoggedInUser");
            Favorite Favorite = new Favorite();
            Favorite.RecipeId = RecipeId;
            Favorite.UserId = fromLogin.UserId;
            dbContext.Add(Favorite);
            dbContext.SaveChanges();
            return RedirectToAction("SavedRecipes");
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
