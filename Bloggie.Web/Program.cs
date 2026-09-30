using Bloggie.Web.Data;
using Bloggie.Web.Repositories;
using Imagekit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BloggieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BloggieDbConnectionString"))
);

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BloggieAuthDbConnectionString"))
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.Configure<IdentityOptions>(option =>
{
    option.Password.RequireDigit = true;
    option.Password.RequireLowercase = true;
    option.Password.RequireNonAlphanumeric = true;
    option.Password.RequireUppercase = true;
    option.Password.RequiredLength = 6;
    option.Password.RequiredUniqueChars = 1;
});

builder.Services.AddScoped<ITagRepository,TagRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.Configure<ImageKitOptions>(builder.Configuration.GetSection("ImageKit"));
builder.Services.AddScoped<IImageRepository, ImageKitImageRepository>();
builder.Services.AddScoped<IBlogPostLikeRepository, BlogPostLikeRepository>();
builder.Services.AddScoped<IBlogPostCommentRepository, BlogPostCommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BloggieDbContext>();
    await context.Database.MigrateAsync();

    var AuthContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await AuthContext.Database.MigrateAsync();

}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
