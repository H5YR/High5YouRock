var builder = WebApplication.CreateBuilder(args);

// Add Vite services
builder.Services.AddViteServices();

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();

var app = builder.Build();

// Use Vite middleware in development
if (app.Environment.IsDevelopment())
{
    app.UseViteDevelopmentServer();
}

await app.BootUmbracoAsync();

// ... rest of your Umbraco middleware

await app.RunAsync();