using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhotovotingApp.Models;
using Microsoft.EntityFrameworkCore;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Check if the database file exists in /app/database
        var databaseFilePath = Path.Combine(builder.Environment.ContentRootPath, "database", "photovoting.db");
        if (!File.Exists(databaseFilePath))
        {
            Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "database"));

            // Copy the default database from /app to /app/database
            var defaultDatabasePath = Path.Combine(builder.Environment.ContentRootPath, "photovoting.db");
            File.Copy(defaultDatabasePath, databaseFilePath);
        }

        // Add services to the container.
        builder.Services.AddDbContext<AppDbContext>();

        builder.Services.AddControllersWithViews();

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
            pattern: "{controller=Photo}/{action=Index}/{id?}");

        app.Run();
    }
}
