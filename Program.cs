using EventsTest.Interfaces;
using EventsTest.Services;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<ICalendarService, GoogleCalendarService>();
builder.Services.AddScoped<IDriveService, GoogleDriveService>();
builder.Services.AddScoped<IGoogleGmailService, GoogleGmailService>();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddAuthentication(o =>
{

    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
        .AddCookie()
        .AddGoogleOpenIdConnect(options =>
        {
            options.ClientId = builder.Configuration.GetValue<string>("AUTH:client_id");
            options.ClientSecret = builder.Configuration.GetValue<string>("AUTH:client_secret");
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
