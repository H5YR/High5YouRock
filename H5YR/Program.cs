using H5YR.Core.Data.Interfaces;
using H5YR.Core.Data.Stores;
using H5YR.Core.Services;
using Vite.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add Vite services
builder.Services.AddViteServices();

builder.Services.AddSingleton<IMastodonService, MastodonService>();
builder.Services.AddSingleton<IPostCounterStore, PostCounterStore>();

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();


await app.BootUmbracoAsync();

// Use static files (required for Vite assets in production)
app.UseStaticFiles();

// Use Vite middleware in development
if (app.Environment.IsDevelopment())
{
    app.UseViteDevelopmentServer();
}

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
