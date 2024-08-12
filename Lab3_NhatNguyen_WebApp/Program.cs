using Amazon.Runtime;
using Lab3_NhatNguyen_WebApp.Models;
using Lab3_NhatNguyen_WebApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.LoginPath = "/login";
    option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
});
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var myConfig = builder.Configuration.GetSection("AmazonCredential").Get<AmazonCredential>();
BasicAWSCredentials credential = new BasicAWSCredentials(myConfig.AccessKey, myConfig.SecretKey);
builder.Configuration.AddSystemsManager("/DBAccess", new Amazon.Extensions.NETCore.Setup.AWSOptions() { Credentials = credential,Region = Amazon.RegionEndpoint.CACentral1});
var connectionString = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("Connection2RDS"));
connectionString.UserID = builder.Configuration["DBUsername"];
connectionString.Password = builder.Configuration["DBPassword"];
builder.Services.AddDbContext<MovieWebAppContext>(options => options.UseSqlServer(connectionString.ConnectionString,sqlServeroptions => sqlServeroptions.EnableRetryOnFailure(maxRetryCount:5,maxRetryDelay:TimeSpan.FromSeconds(30),errorNumbersToAdd:null)));
builder.Services.AddSingleton(myConfig);
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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
