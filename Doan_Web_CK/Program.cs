using Doan_Web_CK;
using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddRoles<IdentityRole>();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRazorPages();

builder.Services.AddScoped<IBlogRepository, EFBlogRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<ICommentRepository, EFCommentRepository>();
builder.Services.AddScoped<INotifiticationRepository, EFNofiticationRepository>();
builder.Services.AddScoped<IAccountRepository, EFAccountRepository>();
builder.Services.AddScoped<ILikeRepository, EFLikeRepository>();
builder.Services.AddScoped<IFriendShipRepository, EFFriendShipRepository>();
builder.Services.AddTransient<ISenderEmail, EmailSender>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://localhost:7021/")); // Thay origin tại đây bằng địa chỉ mà bạn muốn cho phép truy cập.
});

builder.Services.AddAuthentication()
    .AddFacebook(op =>
    {
        op.ClientId = "2399387130253931";
        op.ClientSecret = "e93ff81ddb2550deae351b2c79a616d5";
    })
    .AddGoogle(options =>
    {

        options.ClientId = "144668207253-4mn5jc9icnbkvu0uv3a5ml4vchoofit4.apps.googleusercontent.com";
        options.ClientSecret = "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";
    });



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

app.UseCors("AllowSpecificOrigin");

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
    string email2 = "nguyennam11112003@gmail.com";
    string password = "Namproplayer2003@";
    if (await UserManager.FindByEmailAsync(email) == null)
    {
        // seeding
        var admin = new ApplicationUser();
        admin.UserName = email;
        admin.Email = email;
        admin.DateCreated = DateTime.Now;
        admin.ImageUrl = "https://w7.pngwing.com/pngs/178/595/png-transparent-user-profile-computer-icons-login-user-avatars-thumbnail.png";
        await UserManager.CreateAsync(admin, password);
        await UserManager.AddToRoleAsync(admin, "Admin");

    }
    //if (await UserManager.FindByEmailAsync(email2) == null)
    //{
    //    var member = new ApplicationUser();
    //    member.UserName = email2;
    //    member.Email = email2;
    //    member.DateCreated = DateTime.Now;
    //    member.ImageUrl = "https://w7.pngwing.com/pngs/178/595/png-transparent-user-profile-computer-icons-login-user-avatars-thumbnail.png";
    //    await UserManager.CreateAsync(member, password);
    //    await UserManager.AddToRoleAsync(member, "Member");
    //}
}
app.Run();
