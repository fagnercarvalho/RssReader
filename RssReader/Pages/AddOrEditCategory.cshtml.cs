using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RssReader.Data;
using RssReader.Data.Model;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RssReader.Pages
{
    public class AddOrEditCategoryModel : PageModel
    {
        private readonly Context context;

        public AddOrEditCategoryModel(Context context)
        {
            this.context = context;
        }

        [BindProperty]
        public int? CategoryId { get; set; }

        [BindProperty]
        [Required]
        public string Name { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var category = await this.context.FeedCategories.SingleAsync(f => f.Id == id.Value);
                CategoryId = category.Id;
                Name = category.Name;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!CategoryId.HasValue)
            {
                var category = new FeedCategory
                {
                    Name = this.Name
                };

                this.context.FeedCategories.Add(category);
            }
            else
            {
                var existingCategory = await this.context.FeedCategories.SingleAsync(f => f.Id == CategoryId.Value);
                existingCategory.Name = this.Name;

                this.context.Attach(existingCategory).State = EntityState.Modified;
            }

            await this.context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}