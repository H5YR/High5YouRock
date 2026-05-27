using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace H5YR.Core.Data.Entities
{
    [TableName("DiscourseReactions")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class DiscourseReaction
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public int PostId { get; set; }
        
        public string PostUrl { get; set; } = string.Empty;
        
        public string UserName { get; set; } = string.Empty;
        
        public string UserDisplayName { get; set; } = string.Empty;
        
        public string UserAvatar { get; set; } = string.Empty;
        
        public string PostContent { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        
        public string TopicTitle { get; set; } = string.Empty;
        
        public string TopicSlug { get; set; } = string.Empty;
        
        public int ReactionCount { get; set; }
        
        public string ReactingUserName { get; set; } = string.Empty;
        
        public DateTime ReceivedAt { get; set; }
    }
}