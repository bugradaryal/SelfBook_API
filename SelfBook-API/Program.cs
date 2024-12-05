using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using DataAccess;
using Entities.DTOs;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Business.Abstract;
using Business.Concrete;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddDbContext<DBContext>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<DBContext>().AddDefaultTokenProviders();



builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<EmailSender>(builder.Configuration.GetSection("EmailSender"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
   {
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<ITokenServices, TokenManager>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers()
        .AddNewtonsoftJson(options => {
            // CamelCase düzeni kullanarak JSON anahtarlarýný küçük harflerle baþlatmak
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // JSON çýktýsýný okunabilir hale getirmek için
            options.SerializerSettings.Formatting = Formatting.Indented;

            // Hata durumlarý için daha fazla ayrýntý vermek üzere
            options.SerializerSettings.Error = (sender, args) =>
            {
                args.ErrorContext.Handled = true; // Hatalý JSON'u görmezden gelmek
            };
        });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();  // Routing önce olmalý

app.UseAuthentication();  // Authentication middleware'inin önce gelmesi lazým
app.UseAuthorization();   // Authorization middleware'ini sonra kullanmalýsýnýz



app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SelfBook-API");
});

app.MapControllers();

app.Run();
