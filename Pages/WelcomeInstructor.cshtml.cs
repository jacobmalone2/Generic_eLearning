using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CS3750Assignment1.Pages
{
    public class WelcomeInstructorModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public WelcomeInstructorModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int InstructorId { get; set; }

        public bool? pageRole = true; // added for calendar functionality

        public List<Course> InstructorCourses { get; set; } = new List<Course>();

        public void OnGet()
        {
            if (Request.Cookies.ContainsKey("LoggedUserID"))
            {
                InstructorId = Int32.Parse(Request.Cookies["LoggedUserID"]);

                // Fetch all courses where the instructor is teaching
                InstructorCourses = _context.Course
                    .Where(c => c.InstructorID == InstructorId)
                    .ToList();
            }
        }
    }
}
