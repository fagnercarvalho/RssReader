using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RssReader.Data.Model
{
    [Table("feed")]
    public class Feed
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Url]
        [Column("url")]
        public string Url { get; set; }

        [Column("feed_category_id")]
        public int? CategoryId { get; set; }

        public FeedCategory Category { get; set; }
    }
}
