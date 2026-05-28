using H5YR.Core.Data.Constants;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace H5YR.Core.Data.Entities
{
    [TableName(DiscourseReactionSchemaConstants.TableName)]
    [PrimaryKey(DiscourseReactionSchemaConstants.PrimaryKey, AutoIncrement = true)]
    [ExplicitColumns]
    public class DiscourseReaction
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        [Column(DiscourseReactionSchemaConstants.Id)]
        public int Id { get; set; }

        [Column(DiscourseReactionSchemaConstants.PostId)]
        public int PostId { get; set; }
        
        [Column(DiscourseReactionSchemaConstants.PostUrl)]
        public string PostUrl { get; set; } = string.Empty;
        
        [Column(DiscourseReactionSchemaConstants.UserName)]
        public string UserName { get; set; } = string.Empty;
        
        [Column(DiscourseReactionSchemaConstants.UserDisplayName)]
        public string UserDisplayName { get; set; } = string.Empty;
        
        [Column(DiscourseReactionSchemaConstants.UserAvatar)]
        public string UserAvatar { get; set; } = string.Empty;
        
        [Column(DiscourseReactionSchemaConstants.PostContent)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string PostContent { get; set; } = string.Empty;
        
        [Column(DiscourseReactionSchemaConstants.CreatedAt)]
        public DateTime CreatedAt { get; set; }
        
        [Column(DiscourseReactionSchemaConstants.TopicTitle)]
        public string TopicTitle { get; set; } = string.Empty;
        
        [Column(DiscourseReactionSchemaConstants.TopicSlug)]
        public string TopicSlug { get; set; } = string.Empty;
        
        [Column(DiscourseReactionSchemaConstants.ReactionCount)]
        public int ReactionCount { get; set; }
        
        [Column(DiscourseReactionSchemaConstants.ReactingUserName)]
        public string ReactingUserName { get; set; } = string.Empty;
        
        [Column(DiscourseReactionSchemaConstants.ReceivedAt)]
        public DateTime ReceivedAt { get; set; }
    }
}