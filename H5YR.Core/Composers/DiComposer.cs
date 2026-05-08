using H5YR.Core.Data.Interfaces;
using H5YR.Core.Data.Stores;
using H5YR.Core.Services;
using H5YR.Core.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NPoco;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace H5YR.Core.Composers
{
    public class DiComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            const string umbracoDbDSN = Umbraco.Cms.Core.Constants.System.UmbracoConnectionName;

            builder.Services.AddSingleton<IDatabase>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString(umbracoDbDSN);

                return new NPoco.Database(
                    connectionString,
                    DatabaseType.SqlServer2012,
                    SqlClientFactory.Instance);
            });

            builder.Services.AddSingleton<IMastodonService, MastodonService>();
            builder.Services.AddSingleton<IPostCounterStore, PostCounterStore>();
            builder.Services.Configure<APISettings>(builder.Config.GetSection("APISettings"));
            builder.Services.AddSingleton<HomeSettingsService>();

            // Widget H5YR services
            builder.Services.AddSingleton<IWidgetH5yrStore, WidgetH5yrStore>();
            builder.Services.AddSingleton<IWidgetH5yrService, WidgetH5yrService>();
            builder.Services.AddSingleton<IWidgetJwtService, WidgetJwtService>();
            builder.Services.Configure<WidgetSettings>(builder.Config.GetSection("WidgetSettings"));
        }
    }
}
