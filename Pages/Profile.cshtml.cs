using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS3750Assignment1.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public ProfileModel(CS3750Assignment1Context context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Account Account { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                //return NotFound();
                return RedirectToPage("/index");
            }

            var account = await _context.Account.FindAsync(id);

            if (account is not null)
            {
                Account = account;

                return Page();
            }

            return NotFound();
        }
    }
}
