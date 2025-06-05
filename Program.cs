using Hangfire;
using Microsoft.EntityFrameworkCore;
using WUIAM.Models;
using WUIAM.Services.Config.SeedService;
using Hangfire.Dashboard;
using WUIAM.Interfaces;
using WUIAM.Services;
using WUIAM.Repositories.IRepositories;
using WUIAM.Repositories;
using brevo_csharp.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Configure Brevo (SendinBlue) API
var brevoApiKey = builder.Configuration["BrevoApi:ApiKey"];
Configuration.Default.ApiKey.Add("api-key", brevoApiKey);

var brevoApiUrl = builder.Configuration["BrevoApi:ApiUrl"];
if (!string.IsNullOrEmpty(brevoApiUrl))
{
    Configuration.Default.BasePath = brevoApiUrl;
}

// Database
builder.Services.AddDbContext<WUIAMDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Hangfire
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();

// Services & Repositories
builder.Services.AddTransient<SeedService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<INotifyService, NotifyService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Authentication
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
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.ASCII.GetBytes(
                builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.")
            )
        ),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Global authorization policy - protect all routes by default
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

// Optional console logger
builder.Services.AddSingleton<Microsoft.Extensions.Logging.ILoggerProvider, Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>();

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WUIAMDbContext>();
    db.Database.Migrate();
}

// Dev-only Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() }
});

// Enqueue data seed job
BackgroundJob.Enqueue<SeedService>(s => s.Seed());

app.MapControllers();

app.Run();
