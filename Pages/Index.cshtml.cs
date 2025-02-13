using CS3750Assignment1.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using System.Text;
using System.Security.Cryptography;

namespace CS3750Assignment1.Pages {
    public class IndexModel:PageModel {
        private readonly CS3750Assignment1Context _context;
        public bool? pageRole = null; //added for calendar functionality -> null is for pages with no role required, false is for students, true is for instructors

        public IndexModel(CS3750Assignment1Context context) {
            _context = context;
        }

        [BindProperty]
        public string username { get; set; } = string.Empty;

        [BindProperty]
        public string password { get; set; } = string.Empty;

        public void OnGet() {
            try
            {
                Response.Cookies.Delete("LoggedUserID");
            }
            catch (Exception ex)
            {
                Console.WriteLine("No UserID Cookie Found: " + ex);
            }
            try
            {
                Response.Cookies.Delete("LoggedUserRole");
            }
            catch (Exception ex)
            {
                Console.WriteLine("No UserRole Cookie Found: "+ ex);
            }
        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync() {
            if (_context == null) {
                Console.WriteLine("Database context is null.");
                return Page();
            }

            // Log the input values
            Console.WriteLine($"Username: {username}, Password: {password}");

            // Hash the input password
            var hashedPassword = HashPassword(password);

            // Query the database to find the account with the provided username and hashed password
            Account account = await _context.Account
                .Where(a => a.Username == username && a.Password == hashedPassword)
                .FirstOrDefaultAsync();

            // Check if the account exists
            if (account == null || account.Id==0) {
                ModelState.AddModelError(string.Empty,"Invalid username or password.");
                return Page();
            }

            // Grab User ID
            Response.Cookies.Append("LoggedUserID", account.Id.ToString(), new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            // Grab User Role
            Response.Cookies.Append("LoggedUserRole", account.AccountRole);

            // Grab User Profile Picture Path
            if (!string.IsNullOrEmpty(account.imgSource)) {
                Response.Cookies.Append("LoggedUserPFP",account.imgSource);
            }
                //Response.Cookies.Append("LoggedUserPFP", account.imgSource);

            // Cookie function failed
            if (Request.Cookies["LoggedUserID"] == null || Request.Cookies["LoggedUserID"] == "0")
            {
                ModelState.AddModelError(string.Empty,"Cookie failed to set UserID.");
                return Page();
            }

            // Redirect to the applicable Welcome page based on user account
            if (account.AccountRole == "Instructor") {
                return RedirectToPage("./Courses/Index/");
            }
            else if (account.AccountRole == "Student") {
                return RedirectToPage("./Registrations/Index/");
            } // else if the the account is not being found, return to the index page
            else {
                ModelState.AddModelError(string.Empty,"Account role not found.");
                return Page();
            }

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