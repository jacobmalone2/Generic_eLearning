using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Text;
using System.Security.Cryptography;

namespace CS3750Assignment1.Pages
{
    public class SignUp2Model : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public SignUp2Model(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            if (Account.Password != Account.PasswordConfirmation) {
                ModelState.AddModelError("Account.Password","Passwords do not match. Please try again.");
                return Page();
            }

            // Hash the password before saving
            Account.Password = HashPassword(Account.Password);

            _context.Account.Add(Account);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        private string HashPassword(string password) {
            using (var sha256 = SHA256.Create()) {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
