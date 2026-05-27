using H5YR.Core.Data.Entities;

namespace H5YR.Core.Data.Constants
{
    public static class WidgetH5yrSchemaConstants
    {
        public const string TableName = "WidgetH5yr";

        public const string PrimaryKey = Id;

        public const string Id = nameof(Entities.WidgetH5yr.Id);

        public const string AuthProvider = nameof(Entities.WidgetH5yr.AuthProvider);

        public const string ExternalUserId = nameof(Entities.WidgetH5yr.ExternalUserId);

        public const string DisplayName = nameof(Entities.WidgetH5yr.DisplayName);

        public const string AvatarUrl = nameof(Entities.WidgetH5yr.AvatarUrl);

        public const string ProfileUrl = nameof(Entities.WidgetH5yr.ProfileUrl);

        public const string TargetUrl = nameof(Entities.WidgetH5yr.TargetUrl);

        public const string TargetSiteHost = nameof(Entities.WidgetH5yr.TargetSiteHost);

        public const string Content = nameof(Entities.WidgetH5yr.Content);

        public const string CreatedAt = nameof(Entities.WidgetH5yr.CreatedAt);
    }
}
