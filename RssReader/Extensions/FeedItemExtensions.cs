using CodeHollow.FeedReader;
using Humanizer;

namespace RssReader.Extensions
{
    public static class FeedItemExtensions
    {
        public static string GetPublishedDate(this FeedItem feedItem)
        {
            if (!feedItem.PublishingDate.HasValue)
            {
                return string.Empty;
            }

            return feedItem.PublishingDate.Value.Humanize();
        }
    }
}
