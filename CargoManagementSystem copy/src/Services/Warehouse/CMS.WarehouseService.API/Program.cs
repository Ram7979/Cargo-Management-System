using System.Text;
using CMS.Shared.Middleware;
using CMS.WarehouseService.Application;
using CMS.WarehouseService.Domain.Entities;
using CMS.WarehouseService.Infrastructure;
using CMS.WarehouseService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

// Bootstrap logger — catches errors before full Serilog is configured
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CMS Warehouse Service...");

    var builder = WebApplication.CreateBuilder(args);

    // Full Serilog configuration
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "WarehouseService")
        .WriteTo.Console()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
        .WriteTo.Seq(
            ctx.Configuration["Seq:Url"] ?? "http://localhost:5341",
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information));

    // Add services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Swagger with JWT Bearer
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "CMS Warehouse Service", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Bearer token. Enter your token below.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // JWT Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization();
    builder.Services.AddHttpContextAccessor();

    // Register application and infrastructure layers
    builder.Services.AddWarehouseApplication();
    builder.Services.AddWarehouseInfrastructure(builder.Configuration);

    var app = builder.Build();

    // ─── Database migration & seeding ───────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        try
        {
            db.Database.Migrate();
            Log.Information("Warehouse database migrated successfully.");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Migration failed, trying EnsureCreated...");
            try { db.Database.EnsureCreated(); }
            catch { /* ignore if DB already exists */ }
        }

        // Seed default warehouses & bins if none exist
        if (!db.Warehouses.Any())
        {
            Log.Information("Seeding default warehouses and bins...");

            var warehouses = new[]
            {
                Warehouse.Create("NYC Central Warehouse", "123 Logistics Blvd", "New York", "USA", 50000m, 20),
                Warehouse.Create("LAX West Hub", "456 Freight Ave", "Los Angeles", "USA", 75000m, 30),
                Warehouse.Create("Chicago Distribution Center", "789 Cargo St", "Chicago", "USA", 40000m, 15),
                Warehouse.Create("London Gateway Warehouse", "10 Docklands Rd", "London", "UK", 60000m, 25),
                Warehouse.Create("Mumbai Cargo Hub", "Plot 5, JNPT Area", "Mumbai", "India", 35000m, 18)
            };

            db.Warehouses.AddRange(warehouses);
            await db.SaveChangesAsync();

            // Add bins for each warehouse
            var binTemplates = new[] { "A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "D1", "D2" };
            foreach (var wh in warehouses)
            {
                var binCount = Math.Min(wh.TotalBins, binTemplates.Length);
                for (int i = 0; i < binCount; i++)
                {
                    var zone = binTemplates[i][..1]; // A, B, C, D
                    var level = binTemplates[i][1..]; // 1, 2, 3
                    var bin = Bin.Create(wh.Id, $"BIN-{binTemplates[i]}", 2000m + (i * 500m), zone, level);
                    db.Bins.Add(bin);
                }
            }

            // Add special bins
            var nycWarehouse = warehouses[0];
            db.Bins.Add(Bin.Create(nycWarehouse.Id, "COLD-STORAGE-01", 5000m, "COLD", "1"));
            db.Bins.Add(Bin.Create(nycWarehouse.Id, "HAZMAT-01", 3000m, "HAZMAT", "1"));
            db.Bins.Add(Bin.Create(nycWarehouse.Id, "OVERSIZED-01", 10000m, "OVERSIZED", "1"));

            await db.SaveChangesAsync();
            Log.Information("Seeded {Count} warehouses with bins.", warehouses.Length);
        }
    }
    // ────────────────────────────────────────────────────────────────────────

    // Middleware pipeline
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseMiddleware<CorrelationIdMiddleware>();
    // CORS — allows gateway Swagger UI to fetch swagger.json
    app.UseCors();
    // Swagger always enabled — required for gateway aggregation
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("CMS Warehouse Service started. Swagger: http://localhost:5005/swagger");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CMS Warehouse Service failed to start");
}
finally
{
    Log.CloseAndFlush();
}
