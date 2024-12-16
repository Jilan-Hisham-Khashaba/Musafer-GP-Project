using AdminDashBoard.Models;
using Gp.Api.Hellpers;
using GP.APIs.Dtos;
using GP.core.Entities.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace AdminDashBoard.Controllers
{
    public class AllAdminController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;

        public AllAdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users
                .Include(ad => ad.Address)
                .OrderByDescending(u => u.LastLogin) // ترتيب المستخدمين حسب تاريخ الإنشاء
                .ToListAsync();

            var adminUsers = new List<UserView>();

            foreach (var user in users)
            {
                // التحقق مما إذا كان المستخدم لديه دور "admin"
                var isAdmin = await userManager.IsInRoleAsync(user, "admin");
                if (isAdmin)
                {
                    var userView = new UserView
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        AddressCountry = user.Address?.Country,
                        City = user.Address?.City,
                        PhoneNumber = user.PhoneNumber,
                    };
                    adminUsers.Add(userView);
                }
            }

            return View(adminUsers);
        }


        public IActionResult AddAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAdmin(AddAdminView signup, IFormFile file)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.ToDictionary(
            //        kvp => kvp.Key,
            //        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            //    );
            //    return Json(new { success = false, errors });
            //}

            string productImageUrl = null;
            if (file != null && file.Length > 0)
            {
                productImageUrl = DocumentSetting.UploadImage(file, "PictureAdmin");
            }

            var user = new AppUser
            {
                DisplayName = signup.DisplayName,
                Email = signup.Email,
                UserName = signup.Email.Split('@')[0],
                PhoneNumber = signup.PhoneNumber,
                PhotoPicture = productImageUrl,
                LastLogin = DateTime.Now,

            };

            var result = await userManager.CreateAsync(user, signup.Password);
            if (result.Succeeded)
            {
                // Check if the "Admin" role exists, if not, create it
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    IdentityRole role = new IdentityRole("Admin");
                    IdentityResult roleResult = await roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        return Json(new { success = false, errors = new { General = "Failed to create admin role" } });
                    }
                }

                await userManager.AddToRoleAsync(user, "Admin");
                return Json(new { success = true });
            }

            var createErrors = result.Errors.ToDictionary(e => e.Code, e => e.Description);
            return Json(new { success = false, errors = createErrors });
        }

        public IActionResult Delete(string id)
        {
            var request = userManager.FindByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }
        [HttpPost]
       
        public async Task<IActionResult> DeleteAdmin(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    // التعامل مع النجاح - عرض رسالة نجاح أو إعادة توجيه أو أي عملية أخرى بحسب المتطلبات
                    return RedirectToAction("Index"); // توجيه المستخدم إلى العرض بعد حذف المستخدم بنجاح
                }
                else
                {
                    // التعامل مع الفشل - عرض رسالة خطأ أو إعادة توجيه أو أي عملية أخرى بحسب المتطلبات
                    return StatusCode(500, "An error occurred while deleting the admin.");
                }
            }
            else
            {
                // التعامل مع حالة عدم العثور على المستخدم - عرض رسالة خطأ أو إعادة توجيه أو أي عملية أخرى بحسب المتطلبات
                return NotFound("User not found.");
            }
        }


    }
}

