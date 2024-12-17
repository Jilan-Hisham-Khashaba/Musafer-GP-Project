using AdminPanal.Models;
using Emgu.CV.Ocl;
using GP.core.Entities.identity;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Repository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanal.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly StoreContext context;
        private readonly IGenericRepositroy<Shipment> shipmentRepo;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<AppUser> userManager,StoreContext context,IGenericRepositroy<Shipment> shipmentRepo ,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.context = context;
            this.shipmentRepo = shipmentRepo;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users.Select(u=>new UserView
            {
                Id = u.Id,
                UserName=u.UserName,
                DisplayName=u.DisplayName,
                Email =u.Email,
                PhoneNumber=u.PhoneNumber,
                //ShipmentCount = context.shipments.Count(st => st.IdentityUserId == u.Id ),
                //TripCount = context.Trips.Count(st => st.UserId == u.Id )
            }).ToListAsync();

            return View(users);
        }
    }
}
