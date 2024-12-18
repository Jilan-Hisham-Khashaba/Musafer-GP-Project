using AutoMapper;
using Gp.Api.Controllers;
using Gp.Api.Errors;
using GP.Core.Repositories;
using GP.Repository;
using GP.Repository.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GP.APIs.Dtos;
using GP.core.Entities.identity;
using GP.core.Services;
using Gp.Api.Twilio;
using Twilio.TwiML.Messaging;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Caching.Memory;
using Gp.Api.Dtos;
using Gp.Api.Hellpers;
using GP.Core.Entities;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;


namespace GP.APIs.Controllers
{

    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> userManager;
   
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;
        private readonly ISmsMessage smsMessage;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IUserRepository accountRepo;

        public AccountsController(UserManager<AppUser> userManager, IConfiguration configuration, IMapper mapper, IMemoryCache memoryCache, ISmsMessage smsMessage, SignInManager<AppUser> signInManager, ITokenService tokenService, IUserRepository AccountRepo)
        {
            this.userManager = userManager;
           
            this.configuration = configuration;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
            this.smsMessage = smsMessage;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            accountRepo = AccountRepo;
        }

        [HttpPost("login")] //POST :api/accounts/login
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Unauthorized(new ApiResponse(401, "Invalid email or password.")); // Return message for invalid email

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401, "Invalid email or password.")); // Return message for invalid password
            }

            //if (user.TwoFactorEnabled)
            //{
            // var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

            // Generate a random 4-digit verification code
            //var verificationCode = GenerateVerificationCode();

            //// Compose the SMS message
            //var sms = new Core.SMS
            //{
            //    PhoneNumber = user.PhoneNumber,
            //    Body = $"Your verification code is: {verificationCode}"
            //};

            //// Send the SMS using SmsSetting
            //var Sendresult = smsMessage.Send(sms);

            //// Handle the result and return an appropriate response
            //if (Sendresult != null && !string.IsNullOrEmpty(Sendresult.Sid))
            //{
            //    memoryCache.Set(model.Email, verificationCode, TimeSpan.FromMinutes(5));
            user.LastLogin = DateTime.UtcNow; // يُفضل استخدام UTC لتفادي مشاكل التوقيت
            await userManager.UpdateAsync(user);
            return Ok(new
            {
                user = new UserDto()
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    lastLogin = user.LastLogin,
                    Token = await tokenService.CreateTokenAsyn(user, userManager),
                    photo = new PictureUserResolver(configuration).Resolve(user, null, null, null)
                },
                message = "login is success"

            });


            //}
            //else
            //{
            //    return BadRequest("Failed to send verification code");
            //}


        }
        [HttpGet("users")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.FindByEmailAsync(email);
          

            var allUsers = await userManager.Users.ToListAsync();

            
            var users = allUsers
                .Where(u => u.Id != user.Id && !userManager.IsInRoleAsync(u, "admin").Result)
                .ToList();

      
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                Email = u.Email,
               
                photo = new PictureUserResolver(configuration).Resolve(u, null, null, null)
            }).ToList();

            return Ok(userDtos);
        }



        [HttpPost("verify")]
        [Authorize]
        public async Task<IActionResult> VerifyCode(VerificationDto model)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);



            // التحقق من صحة البريد الإلكتروني المستخدم
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                // استرجاع المستخدم من قاعدة البيانات باستخدام البريد الإلكتروني
                var user = await userManager.FindByEmailAsync(userEmail);

                // التأكد من وجود المستخدم
                if (user != null)
                {
                    // استرجاع رمز التحقق من MemoryCache باستخدام البريد الإلكتروني كمفتاح
                    if (memoryCache.TryGetValue(userEmail, out string storedCode))
                    {
                        if (model.VerificationCode == storedCode)
                        {
                            // تم التحقق بنجاح، قم بمسح رمز التحقق من MemoryCache
                            memoryCache.Remove(userEmail);
                            return Ok(" Verification successful");
                        }
                    }

                    return BadRequest("Invalid or expired verification code");
                }
            }

            // في حالة عدم وجود بريد إلكتروني مصرح به أو عدم وجود مستخدم
            return BadRequest("Authorized user or valid email not found.");
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (checkEmailExist(model.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse()
                {
                    Errors = new string[] { "THis Email is Already Exist" }
                });

            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,//Rahma.mohamed@gmail.com
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber

            };

            var Result = await userManager.CreateAsync(user, model.Password);
            if (!Result.Succeeded) return BadRequest(new ApiResponse(400));
            await userManager.SetTwoFactorEnabledAsync(user, true);
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await tokenService.CreateTokenAsyn(user, userManager),



            });
        }
        //[Authorize]
        //[HttpPost("login-2f")]
        //public async Task<IActionResult> VerifyCode(string verificationCode)
        //{
        //    // Get the currently authenticated user
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    var user = await userManager.FindByEmailAsync(email);

        //    if (user == null)
        //    {
        //        // If user is not found, return unauthorized
        //        return Unauthorized();
        //    }

        //    // Check if the verification code provided by the user matches the one generated
        //    if (verificationCode == user.VerificationCode)
        //    {
        //        // If verification code is correct, log the user in
        //        await signInManager.SignInAsync(user, isPersistent: false);

        //        // Clear the verification code from the user entity
        //        user.VerificationCode = null;
        //        await userManager.UpdateAsync(user);

        //        // Return success message
        //        return Ok("Verification successful. User logged in.");
        //    }
        //    else
        //    {
        //        // If verification code is incorrect, return bad request
        //        return BadRequest("Incorrect verification code.");
        //    }
        //}

        private string GenerateVerificationCode()
        {
            var random = new Random();
            var verificationCode = random.Next(1000, 9999).ToString();
            return verificationCode;
        }

        [Authorize]
        [HttpGet("currentuser")] //get : api/accounts/currentuser
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.FindByEmailAsync(email);

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await tokenService.CreateTokenAsyn(user, userManager)
            });

        }

        [HttpGet("checkEmail")]
        public async Task<ActionResult<bool>> checkEmailExist(string email)
        {
            return await userManager.FindByEmailAsync(email) is not null;//true

        }
        #region ForgetPassword

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new { message = "The email field is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                var errorResponse = new
                {
                    message = "Invalid Email"
                };
                return BadRequest(errorResponse);
            }

            try
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var verificationCode = GenerateVerificationCode();

                var email = new Email
                {
                    Title = "Reset Password",
                    Body = $"Your verification code is: {verificationCode}",
                    To = model.Email
                };

                EmailSettings.SendEmail(email);

                // Store verification code in cache
                memoryCache.Set(model.Email, verificationCode, TimeSpan.FromMinutes(5));
                return Ok(new { message = "Success", token });

            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }


        [HttpPost("verifyResetPassword")]
        public async Task<IActionResult> VerifyCodeResetPassword(VerificationDto model)
        {
            var userEmail = User.FindFirstValue(model.Email);

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (memoryCache.TryGetValue(model.Email, out string storedCode))
                    {
                        if (model.VerificationCode == storedCode)
                        {
                            memoryCache.Remove(model.Email);
                            var successResponse = new
                            {
                                message = "Verification successful"
                            };
                            return Ok(successResponse);
                        }
                    }

                    return BadRequest(new { message = "Invalid or expired verification code" });
                }
            }

            // If an authorized user or valid email is not found
            return BadRequest(new { message = "Authorized user or valid email not found." });
        }

        #endregion

        #region Reset Password


        [HttpPost("ResetPassword")]

        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = model.Token;
                    var result = await userManager.ResetPasswordAsync(user, token, model.Password);

                    if (result.Succeeded)
                        return Ok(new { message = "Password reset successfully." });

                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return BadRequest(ModelState);
        }
        #endregion
        #region GoogleLogin 

        // MVC VIDEO
        //public IActionResult GoogleLogin()
        //{
        //    var prop = new AuthenticationProperties
        //    {
        //        RedirectUri = Url.Action("GoogleResponse")
        //    };
        //    return Challenge(prop, GoogleDefaults.AuthenticationScheme);
        //}

        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        //    var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
        //    {
        //        claim.Issuer,
        //        claim.OriginalIssuer,
        //        claim.Type,
        //        claim.Value
        //    });

        //    return Ok(result);
        //}



        //[Authorize]
        //[HttpPost("GoogleLogin")]
        //public async Task<IActionResult> GoogleLoginAsync()
        //{
        //    var prop = new AuthenticationProperties
        //    {
        //        RedirectUri = Url.Action("GoogleResponse")
        //    };
        //    return Challenge(prop, GoogleDefaults.AuthenticationScheme);


        //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        //    if (result?.Succeeded == true)
        //    {
        //        // Process authentication result here
        //        // You can return additional information if needed
        //        return Ok("Authentication successful!");
        //    }
        //    else
        //    {
        //        return BadRequest("Authentication failed.");
        //    }
        //}





        [HttpPost("GoogleLogin")]
        async Task<IActionResult> GoogleLogin()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme); //error

            if (result?.Principal != null && result.Principal.Identity != null && result.Succeeded)
            {
                // User is already authenticated, return some data or message
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

                return Ok(new { result.Properties, Claims = claims });

            }
            else
            {
                // User is not authenticated, initiate Google login process
                var prop = new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleLogin", "Auth", null, Request.Scheme)
                };
                return Challenge(prop, GoogleDefaults.AuthenticationScheme);

                // Return JSON indicating that authentication is required
                //return Unauthorized(new { Error = "Authentication required", RedirectUrl = prop.RedirectUri });

            }
        }





        // Final Final
        //[Authorize]
        //[HttpPost("GoogleLogin")]
        //public async Task<IActionResult> GoogleLogin()
        //{
        //    Attempt to authenticate the user with Google
        //   var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        //    if (result?.Principal != null && result.Succeeded)
        //    {
        //        User is authenticated successfully
        //        var claims = result.Principal.Identities.FirstOrDefault()?.Claims.Select(claim => new
        //        {
        //            claim.Type,
        //            claim.Value
        //        });

        //        Return the authenticated user's claims
        //        return Ok(new { Claims = claims });
        //    }
        //    else
        //    {
        //        User is not authenticated or authentication failed
        //         Initiate the Google login process
        //        var properties = new AuthenticationProperties
        //        {
        //            RedirectUri = Url.Action(nameof(HandleGoogleResponse))
        //        };
        //        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //    }
        //}

        //[HttpGet("HandleGoogleResponse")]
        //public async Task<IActionResult> HandleGoogleResponse()
        //{
        //    Handle the response from Google authentication
        //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        //    if (result?.Succeeded == true)
        //    {
        //        Authentication successful
        //        return Ok("Google authentication successful!");
        //    }
        //    else
        //    {
        //        Authentication failed
        //        return BadRequest("Google authentication failed.");
        //    }
        //}



        #endregion
        [Authorize]
        [HttpPut("profile-picture")]
        public async Task<ActionResult> UpdateProfilePicture([FromForm] IFormFile file)
           {
            // تأكد من أن الملف غير فارغ
            if (file == null || file.Length == 0)
            {
                return BadRequest("Empty file");
            }
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.FindByEmailAsync(email);

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user.Id == null)
            {
                return BadRequest("User not found");
            }

            // العثور على مستخدم بواسطة معرفه
            // var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // تحميل الصورة وتحديث صورة الملف الشخصي
            var productImageUrl = DocumentSetting.UploadImage(file, "Pictures");
            if (!string.IsNullOrEmpty(productImageUrl))
            {
                user.PhotoPicture = productImageUrl;
            }

            // تحديث بيانات المستخدم
            var result = await userManager.UpdateAsync(user);


            // تحويل المستخدم إلى DTO مع تحديث صورة الملف الشخصي باستخدام PictureUserResolver
            var userDto = new PictureUserDto
            {
                // تحديد URL للصورة باستخدام PictureUserResolver
                PictureUser = new PictureUserResolver(configuration).Resolve(user, null, null, null)
            };
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // عودة الاستجابة بنجاح
            return Ok(new { message = "Profile picture updated successfully", userDto });
        }
    
    
        [Authorize]
        [HttpGet("GetPhoto")]
        public async Task<ActionResult<PictureUserDto>> GetUserProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // تحويل المستخدم إلى DTO مع تحديث صورة الملف الشخصي باستخدام PictureUserResolver
            var userDto = new PictureUserDto
            {
                // تحديد URL للصورة باستخدام PictureUserResolver
                PictureUser = new PictureUserResolver(configuration).Resolve(user, null, null, null)
            };

            return Ok(new { userDto });
        }
        #region ChangePassword
        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new[] { "User not found" }
                });
            }

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = result.Errors.Select(e => e.Description)
                });
            }

            // يمكنك هنا تحديث أي معلومات إضافية تحتاج إلى التحديث، مثل DisplayName
            if (!string.IsNullOrEmpty(model.DisplayName))
            {
               
                user.DisplayName = model.DisplayName;
                await userManager.UpdateAsync(user);
            }

            return Ok(new ApiResponse(200, "Password changed successfully"));
        }


        #endregion


        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok("Logged out successfully.");
        }



    }

}
