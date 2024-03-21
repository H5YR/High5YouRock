using System;
using h5yr.Data.Constants;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace h5yr.Data.Entities
{
    [TableName(PostCounterSchemaConstants.TableName)]
    [PrimaryKey(PostCounterSchemaConstants.PrimaryKey, AutoIncrement = true)]
    [ExplicitColumns]
    public class PostCounter
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        [Column(PostCounterSchemaConstants.Id)]
        public int Id { get; set; }

        [Column(PostCounterSchemaConstants.Date)]
        public DateTime Date { get; set; }
        
        [Column(PostCounterSchemaConstants.Quantity)]
        public int Quantity { get; set; }
    }
}
