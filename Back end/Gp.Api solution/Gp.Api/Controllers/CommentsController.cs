using AutoMapper;
using Gp.Api.Dtos;
using Gp.Api.Errors;
using Gp.Api.Hellpers;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Core.Specificatios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using   GP.core.Entities.identity;
using GP.core.Sepecifitction;
using GP.Repository;

namespace Gp.Api.Controllers
{

    public class CommentsController : ApiBaseController
    {
        private readonly IGenericRepositroy<Comments> commentRepo;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;

        public CommentsController(IGenericRepositroy<Comments> CommentRepo, IMapper mapper, UserManager<AppUser> userManager)
        {
            commentRepo = CommentRepo;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [Authorize]
        [HttpPost("AddComment/{userId}")]
        public async Task<IActionResult> AddCommentToUser(string userId, [FromBody] CommentDto commentDto)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);

                var currentUser = await userManager.FindByEmailAsync(email);
                if (currentUser == null)
                {
                    return Unauthorized();
                }


                // التحقق من أن المستخدم المستهدف موجود
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                // إنشاء كائن تعليق
                var comment = mapper.Map<CommentDto, Comments>(commentDto);
                comment.UserId = user.Id;
                //currentUser = await userManager.FindByIdAsync(commentDto.UserIdSender);
                comment.SenderId = currentUser.Id;
                //  commentDto.UserSender = currentUser.UserName ;
                // إضافة التعليق إلى قاعدة البيانات
                commentRepo.AddAsync(comment);
                await commentRepo.SaveChangesAsync();
                var response = new
                {
                    CommentId = comment.Id,

                    Message = "Comment added successfully."
                };

                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [Authorize]
        [HttpGet("ALLComments")]
        public async Task<ActionResult<IEnumerable<CommentDTOFlask>>> GetALLComments()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var existingUser = await userManager.FindByEmailAsync(email);

                if (existingUser == null)
                {
                    return Unauthorized(); // إذا لم يتم العثور على المستخدم المعرف
                }

                var comments = await commentRepo.GetAllAsync();

                if (comments == null)
                {
                    return NotFound(new ApiResponse(404));
                }

                var commentsDtos = new List<CommentDTOFlask>();
                foreach (var comment in comments)
                {
                    var user = await userManager.FindByIdAsync(comment.UserId);
                    if (user != null)
                    {
                        var commentDto = mapper.Map<Comments, CommentDTOFlask>(comment);
                        commentDto.UserId = user.Id;
                        commentDto.userName = user.UserName;
                        // استخدم UserManager للبحث عن اسم المستخدم المرسل
                       

                        commentsDtos.Add(commentDto);
                    }
                }

                return Ok(commentsDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }




        [Authorize]
        [HttpGet("specific_user")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                return Unauthorized(); // إذا لم يتم العثور على المستخدم المعرف
            }

            var comments = await commentRepo.GetAllAsync();

            if (comments == null)
            {
                return NotFound(new ApiResponse(404));
            }

            var commentsDtos = new List<CommentDto>();
            foreach (var comment in comments)
            {
                var user = await userManager.FindByIdAsync(comment.UserId);
                if (user != null && user.Id == existingUser.Id)
                {
                    var commentDto = mapper.Map<Comments, CommentDto>(comment);
                    commentDto.UserId = user.Id;
                    commentDto.userName = user.UserName;
                    // استخدم UserManager للبحث عن اسم المستخدم المرسل
                    var userSender = await userManager.FindByIdAsync(commentDto.UserIdSender);
                    if (userSender != null)
                    {
                        commentDto.UserSender = userSender.UserName;
                    }

                    commentsDtos.Add(commentDto);
                }
            }

            return Ok(commentsDtos);
        }
        [Authorize]
        [HttpDelete]//delete :api/comment

        public async Task<ActionResult<bool>> DeleteComment(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var existingUser = await userManager.FindByEmailAsync(email);
            var comment = await commentRepo.GetByIdAsync(id);
            if (existingUser.Id == comment.UserId)
            {
                return await commentRepo.DeleteAsyncComment(id);
            }
            return BadRequest(new ApiResponse(403, "You are not authorized to delete this comment."));
        }


    }
}
