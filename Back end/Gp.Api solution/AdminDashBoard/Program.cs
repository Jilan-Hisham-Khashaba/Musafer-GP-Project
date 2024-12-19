using AdminDashBoard.Middelwares;
using AdminDashBoard.Profiles;
using Gp.Api.Hellpers;
using GP.core.Entities.identity;
using GP.core.Services;
using GP.Core.Entities;
using AutoMapper;
using GP.Core.Repositories;
using GP.Core.Services;
using GP.Core.Specificatios;
using GP.Repository;
using GP.Repository.Data;
using GP.Repository.Identity;
using GP.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

///*------Database
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;


}).AddEntityFrameworkStores<AppIdentityDbContext>();

/*.AddDefaultTokenProviders()*/
//builder.Services.ConfigureApplicationCookie(config =>
//{
//    config.LoginPath = "/Admin/Login";
//});
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//               .AddCookie("token",options =>
//               {
//                   options.LoginPath = "/Admin/Login";
//                   options.AccessDeniedPath = "/Home/Error";


//               }).AddJwtBearer("Bearer", config =>
//               {

//               });
// Register SignalR
builder.Services.AddSignalR();

// Register services
builder.Services.AddScoped<IChatHub, ChatHubAdapter>();
builder.Services.AddTransient<IOrderService, OrderService>();


builder.Services.AddScoped(typeof(IGenericRepositroy<>), typeof(GenericRepositorty<>));

builder.Services.AddAutoMapper(typeof(MappingProfileee));

    //builder.Services.AddAutoMapper(typeof(MappingProfile));
//builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<ShipmentCountSpecification>();
builder.Services.AddScoped<TripCountSpecification>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IFaceComparisonResultRepository), typeof(FaceComparisonResultRepository));
builder.Services.AddScoped<UserManager<AppUser>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Read the token from the cookie
            context.Token = context.Request.Cookies["token"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped(typeof(IRequestRepository), typeof(RequestRepository));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Admin}/{action=Login}/{id?}");
});

app.Run();