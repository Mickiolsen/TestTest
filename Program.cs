using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestTest.Components;
using TestTest.Components.Account;
using TestTest.Data;
using TestTest.Components.Account;
using TestTest.Components;
using TestTest.Data;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages(); // Razor Pages
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});


if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    var connectionString = builder.Configuration.GetConnectionString("MockConnection")
    ?? throw new InvalidOperationException("MockConnection string 'MockConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Admin 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedRolesAndAdminAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Razor Pages Identity
app.MapRazorPages();
app.MapAdditionalIdentityEndpoints();

app.Run();

async Task SeedRolesAndAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    var roles = new[] { "Admin", "User" };

    //foreach (var role in roles)
    //{
    //    if (!await roleManager.RoleExistsAsync(role))
    //    {
    //        await roleManager.CreateAsync(new IdentityRole(role));
    //        Console.WriteLine($"Role '{role}' created successfully.");
    //    }
    //}

    // Admin
    var adminUsers = new List<ApplicationUser>
    {
        new ApplicationUser { UserName = "admin@tec.dk", Email = "admin@tec.dk", EmailConfirmed = true },
        new ApplicationUser { UserName = "admin2@tec.dk", Email = "admin2@tec.dk", EmailConfirmed = true },
        new ApplicationUser { UserName = "admin3@tec.dk", Email = "admin3@tec.dk", EmailConfirmed = true }
    };

    const string adminPassword = "Admin@123456";

    //foreach (var adminUser in adminUsers)
    //{
    //    var existingUser = await userManager.FindByEmailAsync(adminUser.Email);
    //    if (existingUser == null)
    //    {
    //        var result = await userManager.CreateAsync(adminUser, adminPassword);
    //        if (result.Succeeded)
    //        {
    //            await userManager.AddToRoleAsync(adminUser, "Admin");
    //            Console.WriteLine($"Admin user created: {adminUser.Email}");
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Failed to create user {adminUser.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    //        }
    //    }
    //    else
    //    {
    //        if (!await userManager.IsInRoleAsync(existingUser, "Admin"))
    //        {
    //            await userManager.AddToRoleAsync(existingUser, "Admin");
    //            Console.WriteLine($"Added 'Admin' role to existing user: {adminUser.Email}");
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Admin user already exists and in role: {adminUser.Email}");
    //        }
    //    }
    //}
}
