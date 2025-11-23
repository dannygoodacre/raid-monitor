using Microsoft.EntityFrameworkCore;
using RaidMonitor.Application;
using RaidMonitor.Configuration;
using RaidMonitor.Data;
using RaidMonitor.Email;

namespace RaidMonitor.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables();

        builder.Services.AddApplication();
        builder.Services.AddData(builder.Configuration);
        builder.Services.AddEmailServices(builder.Configuration);
        builder.Services.AddHostedServices();
        builder.Services.AddOptions(builder.Configuration);
        builder.Services.AddWeb();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();

            app.UseSwagger();

            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "RaidMonitor v0.1");
                x.RoutePrefix = string.Empty;
            });
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseEndpoints(x => x.MapControllers());

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapRazorPages().WithStaticAssets();

        app.Run();
    }
}
