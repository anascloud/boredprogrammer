using Microsoft.EntityFrameworkCore;
using BoredProgrammer.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LiveConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@gmail.com";
    string adminPassword = "admin123";

    var existingAdminRole = await _roleManager.FindByNameAsync("Admin");
    if (existingAdminRole == null)
    {
        var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
        if (!roleResult.Succeeded)
        {
            throw new Exception($"Failed to create Admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
        }
    }

    var existingAdminUser = await _userManager.FindByEmailAsync(adminEmail);
    if (existingAdminUser == null)
    {
        var newUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail
        };
        var userResult = await _userManager.CreateAsync(newUser, adminPassword);
        if (!userResult.Succeeded)
        {
            throw new Exception($"Failed to create admin user: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
        }
        var addRoleResult = await _userManager.AddToRoleAsync(newUser, "Admin");
        if (!addRoleResult.Succeeded)
        {
            throw new Exception($"Failed to add admin user to role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Post}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
