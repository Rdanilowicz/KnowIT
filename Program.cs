using KnowIT.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Pomelo.EntityFrameworkCore.MySql;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<KnowledgeDbContext>(options =>
                options.UseSqlite("Data Source=knowledge.db"));

builder.Services.AddRazorPages();

//builder.Services.AddDbContext<KnowledgeDbContext>(options =>
//    options.UseMySQL(
//        builder.Configuration.GetConnectionString("MySqlConnection")
//        //new MySqlServerVersion(new Version(8, 0, 35))
//));

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=KB}/{action=Index}/{id?}");

app.Run();
