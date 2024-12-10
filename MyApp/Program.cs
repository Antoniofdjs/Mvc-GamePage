using DotNetEnv;
using MyApp.Data;
using MyApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from the .env file
Env.Load();

// Register HttpClient in the DI container
builder.Services.AddHttpClient();

// Register TwitchTokenManager as a Singleton, passing the HttpClient instance
builder.Services.AddSingleton<TwitchTokenManager>(provider =>
{
// Get the HttpClient instance from the DI container
var httpClient = provider.GetRequiredService<HttpClient>();

// Return a new TwitchTokenManager instance with the HttpClient
return new TwitchTokenManager(httpClient);
});

// Local db contains stores
builder.Services.AddSingleton<LocalData>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
