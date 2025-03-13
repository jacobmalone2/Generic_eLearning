using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

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

        public async Task<IActionResult> OnGetAsync(int? assignmentId)
        {
            if (_context.Submission == null)
            {
                return NotFound();
            }

            IQueryable<Submission> query = _context.Submission
            .Include(s => s.Assignment); // Load Assignment data to get MaxPoints



            if (assignmentId != null)
            {
                query = query.Where(s => s.AssignmentID == assignmentId);
            }

            Submissions = await query.ToListAsync();

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

            return RedirectToPage(new { assignmentId = submission.AssignmentID });
        }


    }
}
