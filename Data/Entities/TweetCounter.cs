using System;
using h5yr.Data.Constants;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace h5yr.Data.Entities
{
    [TableName(TweetCounterSchemaConstants.TableName)]
    [PrimaryKey(TweetCounterSchemaConstants.PrimaryKey, AutoIncrement = true)]
    [ExplicitColumns]
    public class TweetCounter
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        [Column(TweetCounterSchemaConstants.Id)]
        public int Id { get; set; }

        [Column(TweetCounterSchemaConstants.Date)]
        public DateTime Date { get; set; }
        
        [Column(TweetCounterSchemaConstants.Quantity)]
        public int Quantity { get; set; }
    }
}
