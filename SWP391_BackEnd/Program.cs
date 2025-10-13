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
using Repositories.Repositories.StationScheduleRepo;
using Services.Services.StationScheduleService;
using Services.Services.RatingService;
using Repositories.Repositories.RatingRepo;
using Repositories.Repositories.ReportRepo;
using Services.Services.ReportService;
using Repositories.Repositories.BatteryReportRepo;
using Services.Services.BatteryReportService;
using Repositories.Repositories.BssStaffRepo;
using Repositories.Repositories.BatteryHistoryRepo;
using Repositories.Repositories.Dashboard;
using Repositories.Repositories.ExchangeBatteryRepo;
using Services.Services.Dashboard;
using Services.Services.ExchangeBatteryService;
using Services.Payments;

//*************** I KNEW YOU WERE HERE ***************//

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
//*************** I KNEW YOU WERE HERE ***************//

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
//*************** I KNEW YOU WERE HERE ***************//

//Add config PayOS
// Bind config
builder.Services.Configure<PayOSConfig>(builder.Configuration.GetSection("PayOS"));
var payOSConfig = builder.Configuration.GetSection("PayOS").Get<PayOSConfig>();
//*************** I KNEW YOU WERE HERE ***************//

// Đăng ký PayOS SDK
builder.Services.AddSingleton(new PayOS(payOSConfig.ClientId, payOSConfig.ApiKey, payOSConfig.ChecksumKey));
//*************** I KNEW YOU WERE HERE ***************//

// Đăng ký helper verify
builder.Services.AddSingleton<PayOSHelper>(new PayOSHelper(payOSConfig));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
//*************** I KNEW YOU WERE HERE ***************//

// Add DbContext
builder.Services.AddDbContext<SwapXContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//*************** I KNEW YOU WERE HERE ***************//

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();
//*************** I KNEW YOU WERE HERE ***************//

// Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//*************** I KNEW YOU WERE HERE ***************//

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
//*************** I KNEW YOU WERE HERE ***************//

// Register Repositories
builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IEvDriverRepo, EvDriverRepo>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStationRepo, StationRepo>();
builder.Services.AddScoped<IFormRepo, FormRepo>();
builder.Services.AddScoped<IBatteryRepo, BatteryRepo>();
builder.Services.AddScoped<IPackageRepo, PackageRepo>();
builder.Services.AddScoped<IVehicleRepo, VehicleRepo>();
builder.Services.AddScoped<IStationScheduleRepo, StationScheduleRepo>();
builder.Services.AddScoped<IRatingRepo, RatingRepo>();
builder.Services.AddScoped<IReportRepo, ReportRepo>();
builder.Services.AddScoped<IBatteryReportRepo, BatteryReportRepo>();
builder.Services.AddScoped<IBssStaffRepo, BssStaffRepo>();
builder.Services.AddScoped<IBatteryHistoryRepo, BatteryHistoryRepo>();
builder.Services.AddScoped<IExchangeBatteryRepo, ExchangeBatteryRepo>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

//*************** I KNEW YOU WERE HERE ***************//

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
builder.Services.AddScoped<IStationScheduleService, StationScheduleService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IBatteryReportService, BatteryReportService>();
builder.Services.AddScoped<IExchangeBatteryService, ExchangeBatteryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
//*************** I KNEW YOU WERE HERE ***************//

//Register Helper
builder.Services.AddScoped<AccountHelper>();

builder.Services.AddControllers();
//*************** I KNEW YOU WERE HERE ***************//

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
//*************** I KNEW YOU WERE HERE ***************//

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
    // Custom response for unauthorized requests
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                isSuccess = false,
                responseCode = "UNAUTHORIZED",
                message = "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn."
            });
            return context.Response.WriteAsync(result);
        }
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
//*************** I KNEW YOU WERE HERE ***************//

// Middleware bắt lỗi 403
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            isSuccess = false,
            responseCode = "FORBIDDEN",
            message = "Bạn không có quyền truy cập tài nguyên này."
        });
        await context.Response.WriteAsync(result);
    }
});

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//*************** I KNEW YOU WERE HERE ***************//

app.UseHttpsRedirection();
//*************** I KNEW YOU WERE HERE ***************//

app.UseCors("AllowAll");
app.UseCors("AllowAllOrigins");
//*************** I KNEW YOU WERE HERE ***************//

app.UseSession();
//*************** I KNEW YOU WERE HERE ***************//

app.UseAuthentication();
app.UseAuthorization();


//*************** I KNEW YOU WERE HERE ***************//

app.MapControllers();
//*************** I KNEW YOU WERE HERE ***************//

app.Run();
//*************** I KNEW YOU WERE HERE ***************//
