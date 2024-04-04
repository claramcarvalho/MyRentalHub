using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;
using RentalProperties;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connection = builder.Configuration.GetConnectionString("RentalPropertiesDB");
builder.Services.AddDbContext<RentalProperties.DATA.RentalPropertiesDBContext>(options => options.UseSqlServer(connection));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//Adding Cookie Authentication Handler
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.LoginPath = "/Identity/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeOwnerOrAdministrator", policy => 
        policy.RequireAssertion( context => 
            context.User.HasClaim( c => 
                c.Type == "Type" && 
                (
                    c.Value == "PropertyOwner" ||
                    c.Value == "Administrator"
                ))));
    options.AddPolicy("CantBeTenant", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                c.Type == "Type" &&
                (
                    c.Value == "PropertyOwner" ||
                    c.Value == "Administrator" ||
                    c.Value == "Manager"
                ))));
    options.AddPolicy("MustBeManager", policy => policy.RequireClaim("Type", "Manager"));
    options.AddPolicy("MustBeTenant", policy => policy.RequireClaim("Type", "Tenant"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
