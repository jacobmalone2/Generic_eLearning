using CS3750Assignment1.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using NuGet.Protocol;


namespace CS3750Assignment1.Pages {
    public class IndexModel : PageModel {
        private readonly CS3750Assignment1Context _context;

        public IndexModel(CS3750Assignment1Context context) {
            _context = context;
        }

        [BindProperty]
        public string username { get; set; }

        [BindProperty]
        public string password { get; set; }

        public void OnGet() {
        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync() {
            //if (!ModelState.IsValid)
            //{
             //   return Page();
            //}

            // Log the input values
            Console.WriteLine($"Username: {username}, Password: {password}");

            // Query the database to find the account with the provided username and password
            var account = await _context.Account
                .Where(a => a.Username == username && a.PasswordConfirmation == password)
                .FirstOrDefaultAsync();

            // Check if the account exists
            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return Page();
            }

            // Log the found user ID
            Console.WriteLine($"Found User ID: {account.Id}");

            // Redirect to the WelcomeUser page with the found user ID
            return RedirectToPage("./WelcomeUser", new { id = account.Id });
        }
    }
}