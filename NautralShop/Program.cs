using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NaturalShop.Models;
using NautralShop.Models;
using NaturalShop.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddMvcCore();
builder.Services.AddDbContext<NaturalShopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("NaturalShop"));
});

//add sesstion and cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Login/login";
        option.ExpireTimeSpan = TimeSpan.FromHours(1);
        option.AccessDeniedPath = "/Home/Error404";
    });
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("AdminAndEmployee", policy =>
        policy.RequireRole("Employee"));
});

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromDays(1);
});

builder.Services.AddTransient<Mail>();

builder.Services.AddHttpContextAccessor();

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

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
