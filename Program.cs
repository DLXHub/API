using System.Reflection;
using System.Text;
using API.Features.Languages.Extensions;
using API.Shared.Behaviors;
using API.Shared.Extensions;
using API.Shared.Infrastructure;
using API.Shared.Infrastructure.Caching;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Audit Interceptor
builder.Services.AddScoped<AuditableEntitySaveChangesInterceptor>();

// Configure Redis
var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisConfiguration>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConfig?.Configuration;
    options.InstanceName = redisConfig?.InstanceName;
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConfig?.Configuration ?? "localhost:6379"));

builder.Services.AddScoped<ICacheService>(sp =>
{
    var cache = sp.GetRequiredService<IDistributedCache>();
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    return new RedisCacheService(cache, redis, redisConfig?.InstanceName ?? "SchnellKuendigen:");
});

// Configure PostgreSQL
builder.Services.AddDbContext<API.Shared.Infrastructure.ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(API.Shared.Infrastructure.ApplicationDbContext).Assembly.FullName)
    ));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<API.Shared.Infrastructure.ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? throw new InvalidOperationException("JWT SecretKey is not configured"))
        )
    };
});

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// Add AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Add language services
builder.Services.AddLanguageServices();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Configure path base if needed
if (app.Environment.IsDevelopment())
{
    app.UsePathBase("/scalar/v1");
}

app.UseHttpsRedirection();

// Add authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Use language middleware
app.UseLanguageMiddleware();

// Add custom exception handling middleware
app.UseCustomExceptionHandler();

app.MapControllers();

app.Run();
