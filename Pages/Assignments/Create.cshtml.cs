using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Stripe.Tax;
using Stripe;
using System.Globalization;

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
        public string submissionType { get; set; }

        public string[] Types = new[] { "Text_Entry", "File_Upload"};

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int courseID = int.Parse(Request.Cookies["SelectedCourse"]);

            try
            {
                CreateAssignment(courseID, Assignment.Title, Assignment.MaxPoints, DateOnly.Parse(Assignment.DueDate), DueTime, submissionType);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Page();
            }

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Verifies Then Submits Information To The Database.
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="title"></param>
        /// <param name="points"></param>
        /// <param name="dueDate"></param>
        /// <param name="dueTime"></param>
        /// <param name="submissionType"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void CreateAssignment(int courseID, string title, int points, DateOnly dueDate, string dueTime, string submissionType)
        {
            Assignment assignment = new Assignment();

            if (courseID <= 0)
                throw new ArgumentOutOfRangeException("Parameter is out of range: CourseID");
            else
            {
                Course? Course = _context.Course.Where(c => c.Id == courseID).FirstOrDefault();
                if (Course == null)
                    throw new ArgumentNullException("No Course Found.");
                else
                    assignment.CourseID = courseID;
            }

            if (title == null || title == "")
                throw new ArgumentException("Argument cannot be null: No Assignment Title Provided.");
            else assignment.Title = title;

            if (title == null)
                title = "";
            assignment.Title = title;

            if (points == 0)
                throw new ArgumentException("Argument cannot be null: No Point Value Provided.");
            else assignment.MaxPoints = points;

            if (DateTime.Now.DayOfYear > dueDate.DayOfYear || DateTime.Now.Year > dueDate.Year)
                throw new ArgumentException("Invalid Argument. DueDate Cannot Be In The Past");
            else
                assignment.DueDate = dueDate.ToString("yyyy/MM/dd");

            DateTime temp = new DateTime();
            if (!DateTime.TryParseExact(dueTime, "hh:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out temp))
                throw new ArgumentException("Incorrect DateTime Format. Example: 10:30am");
            else
                assignment.DueTime = dueTime;

            if (submissionType == null || submissionType == "")
                throw new ArgumentException("Argument cannot be null: No Submission Type Provided.");
            else if (submissionType != "File_Upload" && submissionType != "Text_Entry")
                throw new ArgumentException("Invalid Argument: Submission Type Must Be a File_Upload or Text_Entry.");
            else
                assignment.AcceptedFileTypes = submissionType;

            _context.Assignment.Add(assignment);
        }
    }
}
