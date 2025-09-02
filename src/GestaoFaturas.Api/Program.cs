using GestaoFaturas.Api.Data;
using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GestaoFaturas.Api;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting GestaoFaturas API");
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Add service defaults & Aspire components
            builder.AddServiceDefaults();
            
            // Configure Serilog
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/gestao-faturas-.txt", rollingInterval: RollingInterval.Day));

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddRazorPages();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure Entity Framework with PostgreSQL via Aspire
            builder.AddNpgsqlDbContext<ApplicationDbContext>("gestaoFaturas", configureDbContextOptions: options =>
            {
                options.UseSnakeCaseNamingConvention()
                       .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
                       .EnableDetailedErrors(builder.Environment.IsDevelopment());
            });

            // Register Repositories and Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            
            // Register Business Services
            builder.Services.AddScoped<IClientService, ClientService>();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();
            
            // Run database migrations
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }
            
            // Map default endpoints
            app.MapDefaultEndpoints();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();
            app.MapRazorPages();

            Log.Information("GestaoFaturas API started successfully");
            app.Run();
            
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}