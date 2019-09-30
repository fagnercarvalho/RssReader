using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RssReader.Data.Model
{
    [Table("feed_category")]
    public class FeedCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        public IList<Feed> Feeds { get; set; }
    }
}
