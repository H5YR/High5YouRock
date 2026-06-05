using Vite.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add Vite services
builder.Services.AddViteServices();

// Note: Service registrations (MastodonService, WidgetH5yrService, etc.) are handled in DiComposer

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
        u.EndpointRouteBuilder.MapControllers();
    });

await app.RunAsync();
