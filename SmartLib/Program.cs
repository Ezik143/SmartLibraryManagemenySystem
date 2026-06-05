using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Entities;
using SmartLib.Models.Settings;
using SmartLib.Option;
using SmartLib.Repository;
using SmartLib.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "../Frontend"
});

// Add services to the container.

var jwtSecret = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret is missing in configuration.");
var jwtIssuer = builder.Configuration["JWT:ValidIssuer"] ?? throw new InvalidOperationException("JWT:ValidIssuer is missing in configuration.");
var jwtAudience = builder.Configuration["JWT:ValidAudience"] ?? throw new InvalidOperationException("JWT:ValidAudience is missing in configuration.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Token validation parameters for JWT authentication
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,        
    ValidIssuer = jwtIssuer,
    ValidAudience = jwtAudience,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
    ClockSkew = TimeSpan.Zero // Optional: Set clock skew to zero to prevent token expiration issues
};

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddOptions<FineSettings>()
    .Bind(builder.Configuration.GetSection("FineSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();


builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddOptions<SmtpOption>()
    .Bind(builder.Configuration.GetSection(SmtpOption.Smtp))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBorrowRecordRepository, BorrowRecordRepository>();
builder.Services.AddScoped<IFineRepository, FineRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register the overdue check background service
builder.Services.AddHostedService<OverdueService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            throw new InvalidOperationException("JWT configuration is missing in appsettings.json");

        options.TokenValidationParameters = tokenValidationParameters;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("SmartLib API");
    });
}

await SeedRolesAsync(app.Services);

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedRolesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var role in Enum.GetNames<UserRole>())
    {
        if (await roleManager.RoleExistsAsync(role))
        {
            continue;
        }

        var result = await roleManager.CreateAsync(new IdentityRole(role));
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Could not create role '{role}'. {errors}");
        }
    }
}
