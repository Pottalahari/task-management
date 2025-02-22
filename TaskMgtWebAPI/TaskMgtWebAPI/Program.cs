using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskMgtWebAPI.Models;
//using TaskMgtWebAPI.Services;

namespace TaskMgtWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Information("Application started");

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database context configuration
            builder.Services.AddDbContext<TaskMgtDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure CORS to allow any origin, any header, and any method
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          //.AllowAnyMethod()
                          .WithMethods("GET", "POST", "PATCH", "PUT") // Allow all HTTP methods, including PATCH
                );
            });
            //builder.Services.AddHostedService<DeadlineNotificationService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Apply the CORS policy
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            // Run the application
            app.Run();
        }
    }
}
