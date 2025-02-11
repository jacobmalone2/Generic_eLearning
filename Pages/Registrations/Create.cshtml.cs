using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.EntityFrameworkCore;

namespace CS3750Assignment1.Pages.Registrations
{
    public class CreateModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public CreateModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Registration Registration { get; set; } = default!;

        public IList<Course> Courses { get; set; } = default!;
        public IList<Registration> Registrations { get; set; } = default!;

        int studentID;

        public async Task OnGetAsync()
        {
            studentID = int.Parse(Request.Cookies["LoggedUserID"]);

            try
            {
                Courses = await _context.Course.ToListAsync();
            }
            catch (Exception ex)
            {
                Courses = new List<Course>(); // Avoid null reference issues
            }

            try
            {
                Registrations = await _context.Registration.ToListAsync();
            }
            catch (Exception ex)
            {
                Registrations = new List<Registration>(); // Avoid null reference issues
            }
        }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            studentID = int.Parse(Request.Cookies["LoggedUserID"]);

            Registration.StudentID = studentID;
            Registration.CourseID = Id; // Fetch Course ID from register button.

            Registration.Id = 0; // Unless you feel like going down a rabit hole, don't delete this line.

            _context.Registration.Add(Registration);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public bool IsRegistered(int x)
        {
            foreach (Registration r in Registrations)
            {
                if (r.CourseID == x) return true;
            }

            return false;
        }
    }
}
