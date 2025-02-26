using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using static CS3750Assignment1.Pages.Assignments.IndexModel;
using static CS3750Assignment1.Pages.Registrations.IndexModel;
using Microsoft.IdentityModel.Tokens;

namespace CS3750Assignment1.Pages.Assignments
{
    public class IndexModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public IndexModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IList<Assignment> Assignments { get; set; } = default!;
        public IList<AssignmentViewModel> SubmittedAssignments { get; set; } = new List<AssignmentViewModel>();

        public int CourseID { get; private set; }
        public bool IsInstructor { get; private set; }
        public HashSet<int> submissions { get; private set; } = new HashSet<int>();

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

            // Get list of assignments.
            var assignmentList = from a in _context.Assignment select a;
            assignmentList = assignmentList.Where(a => a.CourseID == CourseID);

            // Check if student
            if (!IsInstructor)
            {
                // Get list of submissions
                // LINQ JOIN
                var submissionList = (from a in _context.Assignment
                                      join sub in _context.Submission
                                      on a.Id equals sub.AssignmentID
                                      where sub.StudentID == userId
                                      select new AssignmentViewModel
                                      {
                                          Id = sub.Id,
                                          StudentID = sub.StudentID,
                                          CourseID = a.CourseID,
                                          AssignmentID = a.Id,
                                          Title = a.Title,
                                          Description = a.Description,
                                          EarnedPoints = sub.PointsEarned,
                                          MaxPoints = a.MaxPoints,
                                          SubmittedAt = sub.SubmittedAt
                                      });

                submissionList = submissionList.Where(s => s.StudentID == userId);

                submissions = _context.Submission.Where(s => s.StudentID == userId)
                .Select(s => s.AssignmentID).ToHashSet();

                // Remove submissions from assignment list. Make Submissions their own list.
                assignmentList = assignmentList.Where(s => !submissions.Contains(s.Id));
                submissionList = submissionList.Where(s => (submissions.Contains(s.AssignmentID) && s.CourseID == CourseID));

                // Finalize view data.
                SubmittedAssignments = await submissionList.ToListAsync();
            }


            // Finalize view data.
            Assignments = await assignmentList.ToListAsync();

            return Page();
        }
    }

    // Custom ViewModel to hold Registration and Course details
    public class AssignmentViewModel
    {
        // New fields for Course information
        public int Id { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public int AssignmentID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? EarnedPoints { get; set; }
        public int MaxPoints { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
