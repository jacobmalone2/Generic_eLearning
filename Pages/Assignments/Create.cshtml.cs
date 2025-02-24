using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Assignments
{
    public class CreateModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public CreateModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CourseID"] = new SelectList(_context.Course, "Id", "Location");
            return Page();
        }

        [BindProperty]
        public Assignment Assignment { get; set; } = default!;

        [BindProperty]
        public string DueTime { get; set; }

        [BindProperty]
        public bool Text { get; set; }

        [BindProperty]
        public bool File { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            List<string> filetypes = new List<string>();

            // Check for selected file types.
            if (Text)
            {
                filetypes.Add("Text");
            }
            if (File)
            {
                filetypes.Add("File_Upload");
            }

            //make sure there was at least one file type selected
            if (filetypes.Count > 0)
            {
                Assignment.AcceptedFileTypes = string.Join(",", filetypes); //very proud of this
            }
            else
            {
                Assignment.AcceptedFileTypes = "NA";
            }

            Assignment.CourseID = int.Parse(Request.Cookies["SelectedCourse"]);
            Assignment.DueTime = DueTime;

            _context.Assignment.Add(Assignment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
