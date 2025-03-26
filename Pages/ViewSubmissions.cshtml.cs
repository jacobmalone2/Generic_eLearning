using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using CS3750Assignment1.Classes;

namespace CS3750Assignment1.Pages.Submissions
{
    public class ViewSubmissionsModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public ViewSubmissionsModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        public List<Submission> Submissions { get; set; } = new List<Submission>();

        public List<GradedSubmission> GradedSubmissions { get; set; }

        public int MaxScore { get; set; }

        public async Task<IActionResult> OnGetAsync(int? courseId, /*added for individual assignment functionality*/int? assignId)
        {
            if (_context.Submission == null)
            {
                return NotFound();
            }

            IQueryable<Submission> query = _context.Submission
                .Include(s => s.Assignment);

            if (courseId != null && assignId != null)
            {
                query = query.Where(s => s.Assignment.CourseID == courseId && s.AssignmentID == assignId);
            }

            Submissions = await query.ToListAsync();
            if (Submissions.Count > 0)
            {
                //if there is any number of submissions, load the max score of the assignment
                MaxScore = Submissions[0].Assignment.MaxPoints;
            }
            GradedSubmissions = new List<GradedSubmission>();
            foreach (var submission in Submissions)
            {
                //if the submission has earned points, add it to the list of graded assignments
                if (submission.PointsEarned is not null)
                {
                    //cast the nullable int to an int now that we know it's not null
                    int score = (int) submission.PointsEarned;
                    string id = submission.StudentID.ToString();
                    Console.WriteLine(id + " " + score);
                    GradedSubmissions.Add(new GradedSubmission(id, score));
                }
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int SubmissionId, int PointsEarned)
        {
            var submission = await _context.Submission
                .Include(s => s.Assignment) // Load assignment to get MaxPoints
                .FirstOrDefaultAsync(s => s.Id == SubmissionId);

            if (submission == null)
            {
                return NotFound();
            }

            // Ensure EarnedPoints do not exceed MaxPoints
            if (PointsEarned > submission.Assignment.MaxPoints)
            {
                ModelState.AddModelError("", "Earned points cannot exceed Max Points.");
                return Page();
            }

            submission.PointsEarned = PointsEarned;
            await _context.SaveChangesAsync();

            return RedirectToPage("/ViewSubmissions", new { courseId = submission.Assignment.CourseID, assignId = submission.Assignment.Id });
        }




    }

}
