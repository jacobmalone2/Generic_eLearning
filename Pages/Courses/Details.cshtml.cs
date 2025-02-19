using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Courses
{
    public class DetailsModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public DetailsModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        public Course Course { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Course.FindAsync(id);
            if (course == null) return NotFound();

            int loggedInInstructorID = int.Parse(Request.Cookies["LoggedUserID"]);
            if (course.InstructorID != loggedInInstructorID)
            {
                return Unauthorized(); // Prevents deletion by other instructors
            }

            _context.Course.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToPage("/WelcomeInstructor");
        }

    }
}
