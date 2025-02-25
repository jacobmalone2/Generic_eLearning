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

        public async Task<IActionResult> OnGetAsync(int assignmentId)
        {
            if (_context.Submission == null)
            {
                return NotFound();
            }

            Submissions = await _context.Submission
                .Where(s => s.AssignmentID == assignmentId)
                .ToListAsync();

            return Page();
        }
    }
}
