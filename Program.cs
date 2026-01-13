using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WalletAdmin.Data;
using WalletAdmin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //options.UseNpgsql(connectionString));

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

string connectionString;

if (string.IsNullOrEmpty(databaseUrl))
{
    // LOCAL DEVELOPMENT
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}
else
{
    // RAILWAY / PRODUCTION
    var uri = new Uri(databaseUrl);

    var username = uri.UserInfo.Split(':')[0];
    var password = uri.UserInfo.Split(':')[1];

    connectionString =
        $"Host={uri.Host};" +
        $"Port={uri.Port};" +
        $"Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={username};" +
        $"Password={password};" +
        $"SslMode=Require;" +
        $"Trust Server Certificate=true;";
}


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<TransactionService>();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
