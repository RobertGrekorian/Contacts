using Contacts.Controllers.SignalRHub;
using Contacts.Repositories;
using Contacts.Services;
using ContactsData.Data;
using ContactsData.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

//builder.Services.AddSignalR(options =>
//{
//    options.EnableDetailedErrors = true;
//    options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
//});



var connectionString = builder.Configuration.GetConnectionString("SQL_Local");

builder.Services.AddDbContext<ApplicationDbContext>(options=> options.UseSqlServer(connectionString));

// Configure Identity
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()  // Add this if you need role support
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager<SignInManager<ApplicationUser>>()
.AddUserManager<UserManager<ApplicationUser>>()
.AddDefaultTokenProviders();


// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();

// Repositories
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ISharedContactRepository, SharedContactRepository>();
builder.Services.AddScoped<IDapperContactRepository, DapperContactRepository>();
//Services
builder.Services.AddScoped<IContactService, ContactService>();


//Email Sender
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("EmailSettings"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

//// Map SignalR Hub with custom options
//app.MapHub<ChatHub>("/chathub", options =>
//{
//    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;

//});
app.Run();




