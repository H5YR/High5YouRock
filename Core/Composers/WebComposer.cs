using h5yr.Data.Interfaces;
using h5yr.Data.Stores;
using Microsoft.Data.SqlClient;
using NPoco;
using Umbraco.Cms.Core.Composing;

namespace h5yr.Core.Composers
{
    public class WebComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IPostCounterStore, PostCounterStore>();
            
            const string umbracoDbDSN = Umbraco.Cms.Core.Constants.System.UmbracoConnectionName;

            var dbBuilder = WebApplication.CreateBuilder();
            var connectionString = dbBuilder.Configuration.GetConnectionString(umbracoDbDSN);

            builder.Services.AddSingleton<IDatabase>(x =>
            {
                return new NPoco.Database(
                    connectionString,
                    DatabaseType.SqlServer2012,
                    SqlClientFactory.Instance);
            });

        }
    }
}
