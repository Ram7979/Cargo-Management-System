using System.Security.Claims;
using System.Text;
using CMS.IdentityService.Application;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Infrastructure;
using CMS.IdentityService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using CMS.Shared.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

// Bootstrap logger — catches errors before full Serilog is configured
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CMS Identity Service...");

    var builder = WebApplication.CreateBuilder(args);

    // Full Serilog configuration
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "IdentityService")
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
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "CMS Identity Service", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Enter your JWT token (without 'Bearer ' prefix)",
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

    // ASP.NET Core Identity Setup
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false; // As per Prompt 1.1: confirmation disabled or handled
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secret = jwtSettings["Secret"]!;
    
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

    builder.Services.AddAuthorization();
    builder.Services.AddHttpContextAccessor();

    // CORS policy that allows http://localhost:4200 with credentials
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // Register application and infrastructure layers
    builder.Services.AddIdentityApplication();
    builder.Services.AddIdentityInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Middleware pipeline
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseCors("AllowAngular");
    app.UseAuthentication();
    app.UseAuthorization();

    // Swagger — always enabled
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS Identity Service v1");
        c.RoutePrefix = "swagger";
    });

    app.MapControllers();

    // Seed Data
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<IdentityDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            await DatabaseSeeder.SeedRolesAndAdminAsync(roleManager, userManager);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the database.");
        }
    }

    Log.Information("CMS Identity Service started. Swagger: http://localhost:5001/swagger");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CMS Identity Service failed to start");
}
finally
{
    Log.CloseAndFlush();
}
