
using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskMgtWebAPI.Models;

namespace TaskMgtWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            //configure serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Information("started");
                        

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<TaskMgtDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
