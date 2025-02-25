using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS3750Assignment1.Pages
{
    public class WelcomeStudentModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public WelcomeStudentModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int StudentId { get; set; }

        public bool? pageRole = false; // Added for calendar functionality

        public List<Course> RegisteredCourses { get; set; } = new List<Course>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Get Student ID from cookie
            if (!int.TryParse(Request.Cookies["LoggedUserID"], out int StudentId))
            {
                return RedirectToPage("/Index"); // Redirect to login page if no valid student ID
            }

            // Fetch the courses that the student is registered for
            RegisteredCourses = await _context.Registration
                .Where(r => r.StudentID == StudentId)
                .Include(r => r.Course) // Load course details
                .Select(r => r.Course)
                .ToListAsync();

            return Page();
        }
    }
}
