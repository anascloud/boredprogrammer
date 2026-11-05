using BoredProgrammer.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SyncSyntax.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        //Register
        //Login
        //Logout

        public AuthController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //Check for validation
            if (ModelState.IsValid)
            {
                //Create Identity User object 
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };

                //User Create 
                var result = await _userManager.CreateAsync(user, model.Password);

                //If User Ceated Successfully
                if (result.Succeeded)
                {
                    //If the User Role exist in data base
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, true);

                    return RedirectToAction("Index", "Post");
                }

            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Email or Password is Incorrect");
                    return View(model);
                }

                var signInresult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (!signInresult.Succeeded)
                {
                    ModelState.AddModelError("", "Email or Password is Incorrect");
                    return View(model);
                }

                return RedirectToAction("Index", "Post");

            }

            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Post");
        }
    }
}




