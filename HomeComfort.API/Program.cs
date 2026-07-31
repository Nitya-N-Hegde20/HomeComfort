
using HomeComfort.API.Data;
using HomeComfort.API.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace HomeComfort.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200",
                       "https://homecomforthub-dcfpexdreagmbjbv.centralindia-01.azurewebsites.net",
                       "https://nice-pond-080d28600.7.azurestaticapps.net")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
                
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddSingleton<ServiceBusPublisher>();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMemoryCache();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
               
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAngular");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
