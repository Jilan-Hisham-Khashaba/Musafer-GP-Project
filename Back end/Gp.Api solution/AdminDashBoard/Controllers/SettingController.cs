using AdminDashBoard.Helpers;
using AdminDashBoard.Models;
using AutoMapper;
using Gp.Api.Hellpers;
using GP.APIs.Dtos;
using GP.core.Entities.identity;
using GP.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Security.Claims;

namespace AdminDashBoard.Controllers
{
    public class SettingController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ILogger<SettingController> logger;
        private readonly SignInManager<AppUser> signInManager;

        public SettingController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger<SettingController> logger, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.logger = logger;
            this.signInManager = signInManager;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);

            if (emailClaim == null)
            {
                return Unauthorized();
            }

            var email = emailClaim.Value;
            // تأكد من تضمين بيانات العنوان عند جلب المستخدم
            var user = await userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            // إذا كان العنوان فارغًا، أنشئ كائن عنوان جديدًا
            if (user.Address == null)
            {
                user.Address = new Address();
                user.Address.AppUserId = user.Id; // تأكد من تعيين معرف المستخدم للعنوان
            }

            var userViewModel = new UserView
            {
                Id = user.Id,
                DisplayName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AddressCountry = user.Address?.Country,
                City = user.Address?.City,
                firstName = user.Address?.FirstName,
                lastName = user.Address?.LastName,
                image = new UserImageResolver(configuration).Resolve(user, null, null, null),
            };

            return View(userViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserView
            {
                LastLogin=user.LastLogin,
                Id = user.Id,
                UserName = user.UserName,
                DisplayName = user.UserName, // يمكن تعديلها لاسم العرض
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AddressCountry = user.Address?.Country,
                City = user.Address?.City,
                image = new UserImageResolver(configuration).Resolve(user, null, null, null), // Assuming you have a property for ImageUrl in your AppUser
                // اضافة الحقول الأخرى عند الحاجة
            };

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserView model, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Ensure Address is initialized
            if (user.Address == null)
            {
                user.Address = new Address();
            }

            if (file != null && file.Length > 0)
            {
                string productImageUrl = DocumentSetting.UploadImage(file, "PictureAdmin");
                user.PhotoPicture = productImageUrl;
            }

            // Update user fields from model
       
            user.Address.FirstName = model.firstName;
            user.Address.LastName = model.lastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address.Country = model.AddressCountry;
            user.Address.City = model.City;
            user.Address.NationalId = model.NationalId;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }



    }

}


