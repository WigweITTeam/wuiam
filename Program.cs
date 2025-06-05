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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// add configuration for Brevo (formerly SendinBlue) email service
var brevoApiKey = builder.Configuration["BrevoApi:ApiKey"];
Configuration.Default.ApiKey.Add("api-key", brevoApiKey);

// Optional: Configure base URL if you're not using the default (usually not needed)
var brevoApiUrl = builder.Configuration["BrevoApi:ApiUrl"];
if (!string.IsNullOrEmpty(brevoApiUrl))
{
    Configuration.Default.BasePath = brevoApiUrl;
}

// Add SQL Server configuration
builder.Services.AddDbContext<WUIAMDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfire(options =>
{
    options.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddHangfireServer();

// Register seed service
builder.Services.AddTransient<SeedService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<INotifyService, NotifyService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Microsoft.Extensions.Logging.ILoggerProvider, Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
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

var app = builder.Build();

// Apply pending migrations automatically at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WUIAMDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Add Hangfire Dashboard with dev-friendly auth

// Add Hangfire Dashboard with dev-friendly auth
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() }
});
 
// Enqueue a background job for seeding data
BackgroundJob.Enqueue<SeedService>(s => s.Seed());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
