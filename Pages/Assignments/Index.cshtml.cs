using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Assignments
{
    public class IndexModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public IndexModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IList<Assignment> Assignment { get; set; } = default!;
        public int CourseID { get; private set; }
        public bool IsInstructor { get; private set; }
        public HashSet<int> SubmittedAssignments { get; private set; } = new HashSet<int>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!int.TryParse(Request.Cookies["LoggedUserID"], out int userId))
            {
                return RedirectToPage("/Index");
            }

            string userRole = Request.Cookies["LoggedUserRole"];
            IsInstructor = userRole == "Instructor";

            if (id != null)
            {
                CourseID = id.Value;
            }
            else if (Request.Cookies.ContainsKey("SelectedCourse"))
            {
                CourseID = int.Parse(Request.Cookies["SelectedCourse"]);
            }
            else
            {
                return RedirectToPage("/WelcomeStudent");
            }

            Response.Cookies.Append("SelectedCourse", CourseID.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddMinutes(180)
            });

            Assignment = await _context.Assignment
                .Where(a => a.CourseID == CourseID)
                .ToListAsync();

            SubmittedAssignments = _context.Submission
                .Where(s => s.StudentID == userId)
                .Select(s => s.AssignmentID)
                .ToHashSet();

            return Page();
        }
    }
}
