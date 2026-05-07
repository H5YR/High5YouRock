using Microsoft.Extensions.DependencyInjection;
using NPoco;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace H5YR.Core.Composers
{
    public class WebComposer : IComposer
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
        }
    }
}
