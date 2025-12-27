using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using PmjayAgra.Blazor.Client.ApiClients;
using PmjayAgra.Blazor.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<HomeApiClient>();

// Configure DbContext
var conn = builder.Configuration.GetConnectionString("Default") ?? "Server=DINESHJ\\SQLSERVERDJ;Database=GauravLab;User Id=Rapid;Password=Rapid@123;TrustServerCertificate=True;";
if (!string.IsNullOrEmpty(conn))
{
    builder.Services.AddDbContext<AgraDbContext>(options => options.UseSqlServer(conn));
    builder.Services.AddScoped<AgraDataService>();
}
else
{
    // Fail fast with clear message so DI doesn't produce ambiguous errors later
    throw new InvalidOperationException("Database connection string not configured. Set 'ConnectionStrings:Default' in appsettings or the 'DB_CONNECTION' environment variable.");
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();