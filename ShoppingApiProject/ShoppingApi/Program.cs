using ShoppingApi.BusinessLogic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.SMTP;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    //options.Password.RequiredLength = 10;
    //options.Password.RequiredUniqueChars = 3;
    //option.Password.RequireNonAlphanumeric = false;

    options.SignIn.RequireConfirmedEmail = true;

    //option.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContextPool<ApplicationDBContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ExternalDatabaseConnection")
);
});

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
