using Doan_Web_CK;
using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddRoles<IdentityRole>();

builder.Services.AddRazorPages();

builder.Services.AddScoped<IBlogRepository, EFBlogRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<ICommentRepository, EFCommentRepository>();
builder.Services.AddScoped<INotifiticationRepository, EFNofiticationRepository>();
builder.Services.AddScoped<IAccountRepository, EFAccountRepository>();

builder.Services.AddLogging(builder => builder.AddConsole());

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
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "Admin",
        areaName: "Admin",
        pattern: "Admin/{controller=Admin}/{action=Index}"
    );


    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Blog}/{action=Index}/{id?}");

});
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Member" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
using (var scope = app.Services.CreateScope())
{
    var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    string email = "nhpn2003@gmail.com";
    string password = "Namproplayer2003@";
    if (await UserManager.FindByEmailAsync(email) == null)
    {
        var user = new ApplicationUser();
        user.UserName = email;
        user.Email = email;
        user.DateCreated = DateTime.Now;
        user.ImageUrl = "https://w7.pngwing.com/pngs/178/595/png-transparent-user-profile-computer-icons-login-user-avatars-thumbnail.png";
        await UserManager.CreateAsync(user, password);
        await UserManager.AddToRoleAsync(user, "Admin");
    }
}
app.Run();
