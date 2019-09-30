using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RssReader.Data;
using RssReader.Data.Model;
using RssReader.Extensions;
using RssReader.Model;

namespace RssReader.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Context context;

        public IndexModel(Context context)
        {
            this.context = context;
        }

        public IList<FeedItem> Items { get; set; }

        public IList<FeedCategory> Categories { get; set; }

        public string CurrentName { get; set; }

        [BindProperty]
        public int? CurrentFeedId { get; set; }

        [BindProperty]
        public int? CurrentCategoryId { get; set; }

        [BindProperty]
        public string CurrentCriteria { get; set; }

        public int AllFeedsCountLimit { get; set; } = 100;

        public async Task<IActionResult> OnGetAsync(int? feedId, int? categoryId, string criteria)
        {
            Categories = await context.FeedCategories
                .Include(i => i.Feeds)
                .ToListAsync();

            await GetItemsAsync(feedId, categoryId, criteria);

            return Page();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("./Index", new { feedId = CurrentFeedId, categoryId = CurrentCategoryId, criteria = CurrentCriteria });
        }

        private async Task GetItemsAsync(int? feedId, int? categoryId, string criteria)
        {
            CurrentCriteria = criteria;

            if (feedId.HasValue)
            {
                (Items, CurrentName) = await GetItemsByFeedIdAsync(feedId.Value, criteria);

                CurrentFeedId = feedId;

                return;
            }

            if (categoryId.HasValue)
            {
                (Items, CurrentName) = await GetItemsByCategoryIdAsync(categoryId.Value, criteria);

                CurrentCategoryId = categoryId;

                return;
            }

            Items = await GetAllItems(criteria);

            CurrentName = "All Feeds";
        }

        private async Task<(List<FeedItem>, string)> GetItemsByFeedIdAsync(int feedId, string criteria)
        {
            var dbFeed = await this.context.Feeds
                .SingleAsync(f => f.Id == feedId);

            var feed = await CodeHollow.FeedReader.FeedReader.ReadAsync(dbFeed.Url);

            var feedItems = new List<FeedItem>();
            foreach (var item in feed.Items)
            {
                if (!string.IsNullOrEmpty(criteria))
                {
                    if (!item.Title.Contains(criteria, StringComparison.CurrentCultureIgnoreCase) 
                        && !item.Description.Contains(criteria, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }

                var feedItem = new FeedItem
                {
                    Title = item.Title,
                    Description = item.Description,
                    Link = item.Link,
                    ParsedPublishedDate = item.GetPublishedDate(),
                    PublishedDate = item.PublishingDate
                };

                feedItems.Add(feedItem);
            }

            return (feedItems, dbFeed.Name);
        }

        private async Task<(List<FeedItem>, string)> GetItemsByCategoryIdAsync(int categoryId, string criteria)
        {
            var category = await this.context.FeedCategories
                .Include(c => c.Feeds)
                .Where(c => c.Id == categoryId)
                .SingleAsync();

            var feedItems = new List<FeedItem>();
            foreach (var feed in category.Feeds)
            {
                var (currentItems, _) = await GetItemsByFeedIdAsync(feed.Id, criteria);

                foreach (var item in currentItems)
                {
                    feedItems.Add(item);
                }
            }

            var items = feedItems
                .Where(f => f.PublishedDate != null)
                .OrderByDescending(f => f.PublishedDate)
                .ToList();

            return (items, category.Name);
        }

        private async Task<List<FeedItem>> GetAllItems(string criteria)
        {
            var categories = this.context.FeedCategories
                .Include(c => c.Feeds)
                .ToList();

            var feedItems = new List<FeedItem>();
            foreach (var category in categories)
            {
                var (currentItems, _) = await GetItemsByCategoryIdAsync(category.Id, criteria);

                foreach (var item in currentItems)
                {
                    feedItems.Add(item);
                }
            }

            var items = feedItems
                .Where(f => f.PublishedDate != null)
                .OrderByDescending(f => f.PublishedDate)
                .Take(AllFeedsCountLimit)
                .ToList();

            return items;
        }
    }
}
