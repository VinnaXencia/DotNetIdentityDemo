using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotNetBrushUp.Data;
using DotNetBrushUp.Areas.Identity.Data;
using DotNetBrushUp.DataModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DotNetBrushUpDbContextConnection") ?? throw new InvalidOperationException("Connection string 'DotNetBrushUpDbContextConnection' not found.");

builder.Services.AddDbContext<DotNetBrushUpDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<DotNetBrushUpDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

//builder.Services.AddAntiforgery(options =>
//{
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//});

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            //ValidIssuer = "your_issuer", // Replace with your issuer
//            ValidIssuer = @"""https://localhost:7207""", // Replace with your issuer
//            //ValidAudience = "your_audience", // Replace with your audience
//            ValidAudience = @"""https://localhost:7207""", // Replace with your audience
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9C90B77A7A924178B2D7D234E45068F1E308EF8F7436B39FFB1453802E45E65C")) // Replace with your secret key
//        };
//    });

var app = builder.Build();

app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication(); // Add this line for authentication
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=Contacts}/{action=Index}/{id?}");
//app.MapGet("/test", () => "Hello, Test!");

app.MapRazorPages();

app.Run();
