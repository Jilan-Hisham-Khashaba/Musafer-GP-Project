using Emgu.CV.Ocl;
using GP.APIs.Dtos;
using GP.core.Entities.identity;
using GP.core.Services;
using GP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashBoard.Controllers
{
    [AllowAnonymous]
    public class AdminController : Controller
    {
   
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenService tokenService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;

        public AdminController(UserManager<AppUser> userManager, ITokenService tokenService, RoleManager<IdentityRole> roleManager,SignInManager<AppUser> signInManager)
        {
          
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

       
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var user = await userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email is invalid");
                TempData["ErrorMessage"] = "Email is invalid";
                return RedirectToAction(nameof(Login));
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                TempData["ErrorMessage"] = "Email Or Password invaild";
                return RedirectToAction(nameof(Login));
            }

            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError(string.Empty, "You are not authorized");
                TempData["ErrorMessage"] = "You are not authorized";
                return RedirectToAction(nameof(Login));
            }

            var token = await tokenService.CreateTokenAsyn(user, userManager);
            await signInManager.SignInAsync(user, isPersistent: false);

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
            });

            return RedirectToAction("Index", "Home");
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterDto signup)
        {
           
           var user = new AppUser 
           {
               DisplayName = signup.DisplayName,
               Email = signup.Email,//Rahma.mohamed@gmail.com
               UserName = signup.Email.Split('@')[0],
               PhoneNumber = signup.PhoneNumber,
               LastLogin=DateTime.Now,
           };
            var result = await userManager.CreateAsync(user, signup.Password);
            if (result.Succeeded)
            {
               
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    IdentityRole role = new IdentityRole("Admin");
                    IdentityResult roleResult = await roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "You are not Authorize");
                    }
                }
               // var token = await tokenService.CreateTokenAsyn(user, userManager);
                //ViewBag["token"] = token;

                await userManager.AddToRoleAsync(user, "Admin");
                return RedirectToAction(nameof(Login));
            }
            return View(signup);

        }
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }


    }
}
