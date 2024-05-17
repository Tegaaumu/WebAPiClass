using ShoppingApi.BusinessLogic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.SMTP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.Xml;
using ShoppingApi;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

// Add services to the container.

//Newly added AddNewtonsoftJson.
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    //options.Password.RequiredUniqueChars = 3;
    //option.Password.RequireNonAlphanumeric = false;

    options.SignIn.RequireConfirmedEmail = true;

    //option.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.SignIn.RequireConfirmedAccount = true;
}).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders().AddSignInManager();

builder.Services.AddDbContextPool<ApplicationDBContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ExternalDatabaseConnection")
);
});
builder.Services.AddAutoMapper(typeof(Program));
//Added jwt authorization
var check1 = Configuration["AuthSettings:Issuer"];
var check2 = Configuration["AuthSettings:Audince"];
var check3 = Configuration["AuthSettings:Key"];
builder.Services.AddAuthentication(auth =>  
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateLifetime= true,
        ValidIssuer = Configuration["AuthSettings:Issuer"],
        ValidAudience = Configuration["AuthSettings:Audince"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"]!)),
        //RequireExpirationTime = true
    };
});
        //Didnt work
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ShopOwner", policy => policy.Requirements.Add(new ShopOwnerRequirement()));
    options.AddPolicy("Admintration", policy => policy.RequireRole("Admin", "admin", "TegaUmu"));
});
//builder.Services.AddAuthorizationBuilder().AddPolicy("Admin", o =>
//{
//    o.RequireAuthenticatedUser();
//    //o.RequireRole("Admin");
//    o.RequireRole("Admin", "admin");
//});
//addedthis code to allow token from JWT to be read.
builder.Services.AddSwaggerGen(swagger =>{
    swagger.SwaggerDoc("v1", new OpenApiInfo { Version = "v1" });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, Array.Empty<string>()
        }
    });
});
////From Kundavent
//builder.Services.AddMvc(options =>
//{
//    var policy = new AuthorizationPolicyBuilder()
//    .RequireAuthenticatedUser()
//    .Build();
//    options.Filters.Add(new AuthorizeFilter(policy));
//});
        //Didnt work
builder.Services.AddScoped<ApplicationDBContext>();
        //Didnt work
builder.Services.AddTransient<IAuthorizationHandler, ShopOwnerHandler>();
builder.Services.AddScoped<IShoppingRepository, MainShoppingRepository>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
