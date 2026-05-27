using H5YR.Core.Data.Interfaces;
using H5YR.Core.Data.Stores;
using H5YR.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace H5YR.Core.Composers
{
    public class DiscourseComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddScoped<IDiscourseReactionStore, DiscourseReactionStore>();
            builder.Services.AddScoped<IDiscourseService, DiscourseService>();
            builder.Services.AddScoped<IUnifiedFeedService, UnifiedFeedService>();
        }
    }
}