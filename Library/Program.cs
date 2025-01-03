using System.Formats.Asn1;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Library.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionStringFromConfig = builder.Configuration.GetConnectionString("DefaultConnection");

var connectionString = $"Data Source={connectionStringFromConfig};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name : "Admin",
    pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await SeedRoles(app.Services);
await SeedUsers(app.Services);

app.Run();

static async Task SeedRoles(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Staff", "User"};

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

static async Task SeedUsers(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string email = "admin@admin.com";
    string password = "Testing#321";

    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        Console.WriteLine("Creating admin user...");
        user = new IdentityUser { UserName = email, Email = email };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            Console.WriteLine("Admin user created successfully.");
            await userManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            Console.WriteLine("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
    else
    {
        Console.WriteLine("Admin user already exists.");
    }
}