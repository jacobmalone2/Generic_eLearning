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
        public bool? pageRole = null; // added for calendar functionality -> null is for pages with no role required, false is for students, true is for instructors

        public IndexModel(CS3750Assignment1Context context) {
            _context = context;
        }

        [BindProperty]
        public string username { get; set; } = string.Empty;

        [BindProperty]
        public string password { get; set; } = string.Empty;

        public void OnGet() {
            try {
                Response.Cookies.Delete("LoggedUserID");
            }
            catch (Exception ex) {
                Console.WriteLine("No UserID Cookie Found: " + ex);
            }
            try {
                Response.Cookies.Delete("LoggedUserRole");
            }
            catch (Exception ex) {
                Console.WriteLine("No UserRole Cookie Found: " + ex);
            }
        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (_context == null)
            {
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
            if (account == null || account.Id == 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return Page();
            }

            // Set User ID cookie
            Response.Cookies.Append("LoggedUserID", account.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddMinutes(180)
            });

            // Set User Role cookie
            Response.Cookies.Append("LoggedUserRole", account.AccountRole);

            // Set User Profile Picture Path cookie (if available)
            if (!string.IsNullOrEmpty(account.imgSource))
            {
                Response.Cookies.Append("LoggedUserPFP", account.imgSource);
            }

            // ? DO NOT CHECK THE COOKIE IN THE SAME REQUEST
            // ? Immediately redirect to force a new request
            if (account.AccountRole == "Instructor")
            {
                Response.Redirect("/WelcomeInstructor");
                return new EmptyResult();
            }
            else if (account.AccountRole == "Student")
            {
                Response.Redirect("/WelcomeStudent");
                return new EmptyResult();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Account role not found.");
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