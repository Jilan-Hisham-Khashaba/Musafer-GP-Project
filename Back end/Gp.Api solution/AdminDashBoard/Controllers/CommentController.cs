using AdminDashBoard.Models;
using Gp.Api.Hellpers;
using GP.core.Entities.identity;
using GP.Core.Entities;
using GP.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using Twilio.Http;

namespace AdminDashBoard.Controllers
{
    public class CommentController : Controller
    {
        private readonly IGenericRepositroy<Comments> commentRepo;
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;

        public CommentController(IGenericRepositroy<Comments> commentRepo, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            this.commentRepo = commentRepo;
            this.userManager = userManager;
            this.configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            var comments = await commentRepo.GetAllAsync();
            var commentViewModels = new List<CommentViewModel>();

            foreach (var comment in comments)
            {
                var receiver = await userManager.FindByIdAsync(comment.SenderId);
                var pictureResolver = new PictureUserResolver(configuration);
                var receiverPhoto = pictureResolver.Resolve(receiver, null, null, null);

                var commentViewModel = new CommentViewModel
                {
                    id = comment.Id, // Make sure to access the correct comment ID
                    receiverphoto = receiverPhoto,
                    CommentText = comment.Content,
                    ReceiverName = receiver != null ? receiver.UserName : "Unknown User"
                };

                commentViewModels.Add(commentViewModel);
            }

            return View(commentViewModels);
        }
        public ActionResult Delete(int id)
        {
            var comment = commentRepo.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {


            var comments = await commentRepo.GetByIdAsync(id);
            if (comments == null)
            {
                return NotFound();
            }

            await commentRepo.DeleteAsync(comments.Id);
            TempData["SuccessMessage"] = "comment deleted successfully!";
            return RedirectToAction(nameof(Index));


        }
    }
}
