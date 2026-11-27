using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrailKit.Application.Services;
using TrailKit.Domain.Entities.Customers;
using TrailKit.Infrastructure.Data;
using TrailKit.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ---- Database (Utsah): SQLite, connection string "TrailKitDb" in appsettings.json ----
builder.Services.AddDbContext<TrailKitDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TrailKitDb")));

// ---- Utsah: UC8 Create Account ----
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IPasswordHasher<Customer>, PasswordHasher<Customer>>();
builder.Services.AddScoped<IAccountService, AccountService>();

// ---- Teammates register their services below ----
// Alison: catalog service / repositories
// Layan:  reservation service / repositories
// Caleb:  rental service / repositories

// ---- Cookie login ----
builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", options =>
{
    options.LoginPath = "/Customers/Login";
    options.AccessDeniedPath = "/Customers/AccessDenied";
});

var app = builder.Build();

// Create/upgrade the database and seed the first manager account.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TrailKitDbContext>();
    db.Database.Migrate();

    var accounts = scope.ServiceProvider.GetRequiredService<IAccountService>();
    await accounts.EnsureDefaultManagerAsync("manager@trailkit.com", "Manager123");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
