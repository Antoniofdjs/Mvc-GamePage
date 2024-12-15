using DotNetEnv;
using MyApp.Data;
using MyApp.Models;
using MyApp.Services.IGDB;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from the .env file
Env.Load();

builder.Services.AddHttpClient();

// Local db contains stores
builder.Services.AddSingleton<LocalData>();

// Register TwitchTokenManager as a Singleton, passing the HttpClient instance
builder.Services.AddSingleton<TwitchTokenManager>(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new TwitchTokenManager(httpClient);
});

// IGDB api service class
builder.Services.AddSingleton<IgdbAPI>(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    var twitchTokenManager = provider.GetRequiredService<TwitchTokenManager>();
    var localData = provider.GetRequiredService<LocalData>();
    return new IgdbAPI(httpClient, twitchTokenManager, localData);
});


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
