using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RssReader.Data;
using RssReader.Data.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RssReader.Pages
{
    public class AddOrEditFeedModel : PageModel
    {
        private readonly Context context;

        public AddOrEditFeedModel(Context context)
        {
            this.context = context;
        }

        [BindProperty]
        public int? FeedId { get; set; }

        [BindProperty]
        [Required]
        public string Name { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Url")]
        public string FeedUrl { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var feed = await this.context.Feeds.SingleAsync(f => f.Id == id.Value);
                FeedId = feed.Id;
                Name = feed.Name;
                FeedUrl = feed.Url;
                CategoryId = feed.CategoryId;
            }

            Categories = context.FeedCategories
                .Select(c =>
                    new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!FeedId.HasValue)
            {
                var feed = new Feed
                {
                    Name = this.Name,
                    Url = this.FeedUrl,
                    CategoryId = this.CategoryId
                };

                this.context.Feeds.Add(feed);
            }
            else
            {
                var existingFeed = await this.context.Feeds.SingleAsync(f => f.Id == FeedId.Value);
                existingFeed.Name = this.Name;
                existingFeed.Url = this.FeedUrl;
                existingFeed.CategoryId = this.CategoryId;

                this.context.Attach(existingFeed).State = EntityState.Modified;
            }

            await this.context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}