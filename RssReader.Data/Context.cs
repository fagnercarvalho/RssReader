using Microsoft.EntityFrameworkCore;
using RssReader.Data.Model;

namespace RssReader.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Feed> Feeds { get; set; }

        public DbSet<FeedCategory> FeedCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedCategory>()
                .HasData(
                    new FeedCategory
                    {
                        Id = 1,
                        Name = "Uncategorized"
                    },
                    new FeedCategory
                    {
                        Id = 2,
                        Name = "Technology"
                    });

            modelBuilder.Entity<Feed>()
                .HasData(
                    new Feed
                    {
                        Id = 1,
                        Name = "Hacker News",
                        Url = "https://news.ycombinator.com/rss",
                        CategoryId = 2
                    },
                    new Feed
                    {
                        Id = 2,
                        Name = "Modus Create",
                        Url = "https://moduscreate.com/feed",
                        CategoryId = 2
                    }, 
                    new Feed
                    {
                        Id = 3,
                        Name = "Reuters",
                        Url = "http://feeds.reuters.com/Reuters/worldNews",
                        CategoryId = 1
                    });
        }
    }
}
