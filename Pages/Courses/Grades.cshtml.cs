using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Courses
{
    public class GradesModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public GradesModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? courseID)
        {
            //assemble lists of students and their grades



            /*ViewData["AssignmentID"] = new SelectList(_context.Assignment, "Id", "DueDate");
            ViewData["StudentID"] = new SelectList(_context.Account, "Id", "AccountRole"); default code*/
            return Page();
        }




        /*
        [BindProperty]
        public Submission Submission { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Submission.Add(Submission);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }more default code, just for referencing at this point*/
    }
}
