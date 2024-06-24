using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CLDV6211_ST10287165_POE_P1.Data;
//using CLDV6211_ST10287165_POE_P1.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CLDV6211_ST10287165_POE_P1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CLDV6211_ST10287165_POE_P1Context") ?? throw new InvalidOperationException("Connection string 'CLDV6211_ST10287165_POE_P1Context' not found.")));

// Add DbContext configuration
builder.Services.AddDbContext<CLDV6211_ST10287165_POE_P1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CLDV6211_ST10287165_POE_P1Context") ?? throw new InvalidOperationException("Connection string 'CLDV6211_ST10287165_POE_P1Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // For example, setting the session timeout to 30 minutes
    options.Cookie.HttpOnly = true; // Enhance security by making the session cookie inaccessible to JavaScript
    options.Cookie.IsEssential = true; // Marks the session cookie as essential for the application to function correctly
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session middleware
app.UseSession();

app.UseAuthorization();

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
