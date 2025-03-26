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
using Stripe;
using Account = CS3750Assignment1.Models.Account;

namespace CS3750Assignment1.Pages
{
    public class SignUp2Model : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;
        public bool? pageRole = null;

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
        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid) 
            {
                return Page();
            }

            try
            {
                CreateAccount(Account.FirstName, Account.LastName, Account.Email, Account.Username, Account.Birthdate
                    , Account.Password, Account.PasswordConfirmation, Account.AccountRole);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Page();
            }

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Verify then submit account information to the database.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="password"></param>
        /// <param name="passwordConfirmation"></param>
        /// <param name="accountRole"></param>
        public void CreateAccount(string firstName, string lastName, string email, string username, DateOnly dateOfBirth, string password, string passwordConfirmation, string accountRole)
        {
            Account account = new();
            bool hasAt = false;
            bool hasDot = false;

            if (firstName == null || firstName == "")
                throw new ArgumentException("Argument cannot be null: No First Name Provided.");
            else  account.FirstName = firstName;

            if (lastName == null || lastName == "")
                throw new ArgumentException("Argument cannot be null: No Last Name Provided.");
            else account.LastName = lastName;

            if (email == null || email == "")
                throw new ArgumentException("Argument cannot be null: No Email Provided.");
            // TODO: Add wildcard support so we can check for the @ and . symbols
            else
            {
                int i = 0;
                for (i = 0; i < email.Length; i++)
                {
                    if (email[i] == '@')
                    {
                        hasAt = true;
                        break;
                    }
                }
                for (int j = i; j < email.Length; j++)
                {
                    if (email[j] == '.')
                    {
                        hasDot = true;
                        break;
                    }
                }
                if (hasAt && hasDot)
                    account.Email = email;
                else
                    throw new ArgumentException("Invalid Argument: Emails contain @ and . symbols, in that order.");
            }

            if (username == null || username == "")
                throw new ArgumentException("Argument cannot be null: No User Name Provided.");
            else account.Username = username;

            account.Birthdate = dateOfBirth;

            if (password == null || password == "")
                throw new ArgumentException("Argument cannot be null: No Password Provided.");
            else if (passwordConfirmation == null || passwordConfirmation == "")
                throw new ArgumentException("Argument cannot be null: No Password Confirmation Provided.");
            else if (password.Length < 8)
                throw new ArgumentException("Invalid Argument: Password must be 8 characters or more.");
            else if (password != passwordConfirmation)
                throw new ArgumentException("Arguments do not match: Password and Confirmation Do Not Match.");
            else
            {
                // Hash the password before saving
                account.Password = HashPassword(password);
                // Erase confirmation now that password is verified.
                passwordConfirmation = "";
            }

            if (accountRole == null || accountRole == "")
                throw new ArgumentException("Argument cannot be null: No Role Provided.");
            else if (accountRole != "Student" && accountRole != "Instructor")
                throw new ArgumentException("Invalid Argument: Account Role Must Be Either Student Or Instructor");
            else account.AccountRole = accountRole;

            _context.Account.Add(account);
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
