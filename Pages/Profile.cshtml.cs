using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CS3750Assignment1.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;
        public bool? pageRole = null;

        public ProfileModel(CS3750Assignment1Context context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Account Account { get; set; } = default!;

        [BindProperty]
        public IFormFile ProfilePicture { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
                //return RedirectToPage("/index");
            }

            var account = await _context.Account.FirstOrDefaultAsync(m => m.Id == id);

            if (account is not null)
            {
                Account = account;
                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostUploadProfilePictureAsync(int id)
        {
            var account = await _context.Account.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            if (ProfilePicture != null)
            {
                var filePath = Path.Combine("wwwroot/images/profile_pictures", ProfilePicture.FileName);

                // Save the file in the directory
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream);
                }

                // Update the account's profile picture path
                account.imgSource = "/images/profiles/profile_pictures/" + ProfilePicture.FileName;

                _context.Account.Update(account);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
