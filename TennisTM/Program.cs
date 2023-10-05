using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TennisTM.Data;
using TennisTM.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TennisTMContextConnection") ?? throw new InvalidOperationException("Connection string 'TennisTMContextConnection' not found.");

builder.Services.AddDbContext<TennisTMDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TennisTMDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
// Injecting DbContext
builder.Services.AddDbContext<TennisTMDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TennisTMContextConnection")));
// Add razor pages
builder.Services.AddRazorPages();

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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Coach" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    string adminEmail = "admin@gmail.com";
    string adminPassword = "Asd@123";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new User();
        admin.UserName = adminEmail;
        admin.Email = adminEmail;
        admin.Name = "Admin";

        await userManager.CreateAsync(admin, adminPassword);
        await userManager.AddToRoleAsync(admin, "Admin");
        await userManager.AddToRoleAsync(admin, "Coach");
    }
}

app.Run();
