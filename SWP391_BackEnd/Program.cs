using BusinessObjects.AppSettings;
using BusinessObjects.Models;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.EvDriverRepo;
using Repositories.Repositories.OrderRepo;
using Repositories.Repositories.StationRepo;
using Services;
using Services.Helpers;
using Services.Services;
using Services.Services.AccountService;
using Services.Services.BatteryService;
using Services.Services.EmailService;
using Services.Services.StationService;
using Services.ServicesHelpers;
using System.Text;
using System.Text.Json.Serialization;
using Account = CloudinaryDotNet.Account;
using Repositories.Repositories.FormRepo;
using Services.Services.FormService;
using Services.Services.PackageService;
using Repositories.Repositories.PackageRepo;
using Repositories.Repositories.VehicleRepo;
using Services.Services.VehicleService;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//Add Cloud setting
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton<Cloudinary>(sp =>
{
    var settings = builder.Configuration
        .GetSection("CloudinarySettings").Get<CloudinarySettings>();

    var account = new Account(
        settings.CloudName,
        settings.ApiKey,
        settings.ApiSecret
    );

    return new Cloudinary(account);
});

//Add config PayOS
// Bind config
builder.Services.Configure<PayOSConfig>(builder.Configuration.GetSection("PayOS"));
var payOSConfig = builder.Configuration.GetSection("PayOS").Get<PayOSConfig>();

// Đăng ký PayOS SDK
builder.Services.AddSingleton(new PayOS(payOSConfig.ClientId, payOSConfig.ApiKey, payOSConfig.ChecksumKey));

// Đăng ký helper verify
builder.Services.AddSingleton<PayOSHelper>(new PayOSHelper(payOSConfig));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


// Add DbContext
builder.Services.AddDbContext<SwapXContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
        });
});
// Register Repositories
builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IEvDriverRepo, EvDriverRepo>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStationRepo, StationRepo>();
builder.Services.AddScoped<IFormRepo, FormRepo>();
builder.Services.AddScoped<IBatteryRepo, BatteryRepo>();
builder.Services.AddScoped<IPackageRepo, PackageRepo>();
builder.Services.AddScoped<IVehicleRepo, VehicleRepo>();
// Register Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IFormService, FormService>();
builder.Services.AddScoped<IBatteryService, BatteryService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();


//Register Helper
builder.Services.AddScoped<AccountHelper>();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SWP391_BackEnd", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

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
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
})
.AddCookie("Cookies")
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseCors("AllowAllOrigins");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
