using KnowIT.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.AspNetCore.Identity;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<KnowledgeDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<KnowledgeDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddRazorPages();

//builder.Services.AddDbContext<KnowledgeDbContext>(options =>
//    options.UseMySQL(
//        builder.Configuration.GetConnectionString("MySqlConnection")
//        //new MySqlServerVersion(new Version(8, 0, 35))
//));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<KnowledgeDbContext>();
    dbContext.Database.OpenConnection();
    dbContext.Database.EnsureCreated();
}

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

// Seed the admin role and user during application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Define the SeedAdminAsync method as a local function
    async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Check if the Admin role exists, and create it if not
        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // Check if an Admin user exists, and create it if not
        const string adminEmail = "admin@knowit.com";
        const string adminPassword = "SecurePassword123!"; // Use a secure password in production!
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
            else
            {
                throw new Exception("Failed to create the admin user: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    // Call the SeedAdminAsync method
    await SeedAdminAsync(services);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

