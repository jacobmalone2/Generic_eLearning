using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Registrations
{
    public class DeleteModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public DeleteModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Registration Registration { get; set; } = default!;


        // Course ID is passed in here, not the registration ID.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || Request.Cookies["LoggedUserID"] == null || Request.Cookies["LoggedUserID"] == "0")
            {
                return NotFound();
            }

            var registration = await _context.Registration.FirstOrDefaultAsync(m => m.CourseID == id && m.StudentID == int.Parse(Request.Cookies["LoggedUserID"]));

            if (registration is not null)
            {
                Registration = registration;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registration.FirstOrDefaultAsync(m => m.CourseID == id && m.StudentID == int.Parse(Request.Cookies["LoggedUserID"]));
            if (registration != null)
            {
                Registration = registration;
                _context.Registration.Remove(Registration);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
