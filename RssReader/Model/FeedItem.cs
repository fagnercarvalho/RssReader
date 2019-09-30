using System;

namespace RssReader.Model
{
    public class FeedItem
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string ParsedPublishedDate { get; set; }
    }
}
