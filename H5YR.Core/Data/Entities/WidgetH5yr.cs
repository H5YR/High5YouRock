using System;
using H5YR.Core.Data.Constants;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace H5YR.Core.Data.Entities
{
    [TableName(WidgetH5yrSchemaConstants.TableName)]
    [PrimaryKey(WidgetH5yrSchemaConstants.PrimaryKey, AutoIncrement = true)]
    [ExplicitColumns]
    public class WidgetH5yr
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        [Column(WidgetH5yrSchemaConstants.Id)]
        public int Id { get; set; }

        [Column(WidgetH5yrSchemaConstants.AuthProvider)]
        [Length(50)]
        public string AuthProvider { get; set; } = string.Empty;

        [Column(WidgetH5yrSchemaConstants.ExternalUserId)]
        [Length(255)]
        public string ExternalUserId { get; set; } = string.Empty;

        [Column(WidgetH5yrSchemaConstants.DisplayName)]
        [Length(255)]
        public string DisplayName { get; set; } = string.Empty;

        [Column(WidgetH5yrSchemaConstants.AvatarUrl)]
        [Length(500)]
        public string AvatarUrl { get; set; } = string.Empty;

        [Column(WidgetH5yrSchemaConstants.ProfileUrl)]
        [Length(500)]
        public string ProfileUrl { get; set; } = string.Empty;

        [Column(WidgetH5yrSchemaConstants.TargetUrl)]
        [Length(2000)]
        public string TargetUrl { get; set; } = string.Empty;

        [Column(WidgetH5yrSchemaConstants.TargetSiteHost)]
        [Length(500)]
        public string TargetSiteHost { get; set; } = string.Empty;

        [Column(WidgetH5yrSchemaConstants.Content)]
        [Length(2000)]
        public string Content { get; set; } = string.Empty;

        [Column(WidgetH5yrSchemaConstants.CreatedAt)]
        public DateTime CreatedAt { get; set; }
    }
}
