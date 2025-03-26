using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public List<Assignment> Assignments { get; set; } = new List<Assignment>(); // Temporary list to hold assignments for each course
        public record AssignmentWithCourse(Assignment Assignment, Course Course); // Structure that holds assignments and courses together
        public List<AssignmentWithCourse> CourseAssignments { get; set; } = new List<AssignmentWithCourse>(); // List of all assignments with their corresponding courses

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

            // Fetch the assignments for each course
            foreach (Course c in RegisteredCourses)
            {
                Assignments = await _context.Assignment //get all assignments for course
                    .Where(a => a.CourseID == c.Id)
                    //.Where(a => DateTime.ParseExact(a.DueDate, "yyyy-mm-dd", CultureInfo.InvariantCulture) > DateTime.Now)
                    //.Include(a => a.Assignment)
                    //.Select(a => a.Assignment)
                    .ToListAsync();

                foreach (Assignment a in Assignments) // add assignments to total list
                {
                    //Added this try/catch block to let me log in as teststudent when testing student assignment graph
                    // was crashing with some strange 2100/05/05 date not being recognized as a datetime object error
                    // 3/25/2025 11:15pm -Carter
                    try
                    {
                        // filter out past assignments
                        if (DateTime.ParseExact(a.DueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) < DateTime.Now) {
                            continue;
                        }
                        AssignmentWithCourse w = new AssignmentWithCourse(a, c);
                        CourseAssignments.Add(w);
                    }
                    catch
                    { continue; }
                } 
            }

            // Sort the list based on due date
            CourseAssignments.Sort((a1, a2) => DateTime.ParseExact(a1.Assignment.DueDate + " " + a1.Assignment.DueTime, "yyyy-MM-dd h:mmtt", CultureInfo.InvariantCulture).CompareTo(DateTime.ParseExact(a2.Assignment.DueDate + " " + a2.Assignment.DueTime, "yyyy-MM-dd h:mmtt", CultureInfo.InvariantCulture)));
            

            return Page();


        }
    }

}
