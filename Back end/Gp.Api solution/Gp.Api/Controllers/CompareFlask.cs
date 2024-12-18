using AutoMapper;
using Gp.Api.Dtos;
using Gp.Api.Errors;
using Gp.Api.Hellpers;
using GP.core.Entities.identity;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using OpenCvSharp;
using System.Security.Claims;
using System.Text;
using Twilio.TwiML.Messaging;

namespace Gp.Api.Controllers
{
    
    public class CompareFlask : ApiBaseController
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client;
        private readonly IGenericRepositroy<Comments> commentRepo;
        private readonly IMapper mapper;
        private readonly IFaceComparisonResultRepository faceComparisonResult;
        private readonly UserManager<AppUser> userManager;
        private readonly FaceComparisonService faceComparison;
        private readonly RecommendedService recommendedService;

        public CompareFlask(IConfiguration configuration, HttpClient client, IGenericRepositroy<Comments> CommentRepo, IMapper mapper, IFaceComparisonResultRepository faceComparisonResult, UserManager<AppUser> userManager, FaceComparisonService faceComparison, RecommendedService recommendedService)
        {
            this.configuration = configuration;
            this.client = client;
            commentRepo = CommentRepo;
            this.mapper = mapper;
            this.faceComparisonResult = faceComparisonResult;
            this.userManager = userManager;
            this.faceComparison = faceComparison;
            this.recommendedService = recommendedService;
        }

        [Authorize]
        [HttpPost("compare-faces")]
        public async Task<IActionResult> CompareFaces(IFormFile image1, bool useCamera = true)
        {
            // Convert image1 to byte array
            byte[] bytes1;
            if (image1 == null)
            {
                return BadRequest(new { message = "Both images are required" });
            }
            using (var ms1 = new MemoryStream())
            {
                await image1.CopyToAsync(ms1);
                bytes1 = ms1.ToArray();
            }
            var email = User.FindFirstValue(ClaimTypes.Email);

            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                byte[] bytes2 = null;
                if (!useCamera)
                {
                    // Capture image2 from the camera and convert it to byte array
                    bytes2 = await faceComparison.CaptureFrameFromCameraAsync();
                }

                // Call the CompareFaces method passing image1 and image2
                var result = await faceComparison.CompareFaces(bytes1, bytes2, useCamera);

                if (result.MatchStatus == "No Match")
                {
                    return BadRequest(new { message = "No match " });
                }
                else
                {
                    
                    var vrefactionFacesDto = new VrefactionFacesDto();

                    

                    if (result.MatchStatus == "No Match")
                    {
                        
                        return BadRequest(new { message = "No match " });
                    }
                    else
                    {
                        // Add the user ID to the result
                        result.userId = existingUser.Id;
                        var mappedVerfication = mapper.Map<VrefactionFacesDto, verficationFaccess>(vrefactionFacesDto);
                        var productImageUrl = DocumentSetting.UploadImage(image1, "flaskApi");
                        if (!string.IsNullOrEmpty(productImageUrl))
                        {
                            mappedVerfication.ImageName = productImageUrl;
                        }
                        var imageResolver = new ImageVerficationResolver(configuration);
                        var resolvedImageUrl = imageResolver.Resolve(mappedVerfication, null, null, null);
                        await faceComparisonResult.SaveComparisonResult(result);
                        return Ok(new { result, imageUrl = resolvedImageUrl });
                    }
                }
                }
            else
            {
                return BadRequest(new { message = "User not found" });
            }
        }

        [Authorize]
        [HttpPost("recommend")]
        public async Task<IActionResult> Recommend()
        {
            try
            {
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
                        commentsDtos.Add(commentDto);
                    }
                }
                var jsonComments = JsonConvert.SerializeObject(commentsDtos);
                var requestContent = new StringContent(jsonComments, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:5001/recommend", requestContent);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var recommendations = JsonConvert.DeserializeObject<IDictionary<string, string>>(responseBody);
                var recommendationList = recommendations.Select(r =>
                {
                    var userId = commentsDtos.FirstOrDefault(c => c.userName == r.Key)?.UserId;
                    return new { UserId = userId, UserName = r.Key, Recommendation = r.Value };
                }).ToList();

                return Ok(new { recommendations = recommendationList });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
    }





