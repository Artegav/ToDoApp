using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using todo_domain_entities.Data;
using todo_domain_entities.Services;

namespace todo_aspnetmvc_ui;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;
        builder.Services.AddDbContext<TodoContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("sqlContext") ??
                throw new InvalidOperationException("Connection string 'sqlContext' not found.")));

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<IListService, ListService>();
        builder.Services.AddScoped<IItemService, ItemService>();

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
            name: "listRoute",
            pattern: "{controller=Todo}/{action=Index}/{id?}");

        app.Run();
    }
}