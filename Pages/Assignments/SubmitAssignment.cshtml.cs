using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Stripe;

namespace CS3750Assignment1.Pages.Submissions
{
    public class SubmitAssignmentModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public SubmitAssignmentModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Submission Submission { get; set; } = default!;

        [BindProperty]
        public string? TextSubmission { get; set; } // For text submissions

        public int CourseID { get; set; }
        public string AllowedFileTypes { get; set; } = ""; // Store Allowed File Types

        [BindProperty(SupportsGet = true)]
        public int AssignmentID { get; set; }

        public Assignment Assignment { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (AssignmentID == 0)
                return NotFound();

#pragma warning disable CS8601 // Possible null reference assignment.
            Assignment = await _context.Assignment
                .Where(a => a.Id == AssignmentID)
                .Select(a => new Assignment
                {
                    Id = a.Id,
                    CourseID = a.CourseID,
                    AcceptedFileTypes = a.AcceptedFileTypes
                })
                .FirstOrDefaultAsync();
#pragma warning restore CS8601 // Possible null reference assignment.

            if (Assignment == null)
                return NotFound();

            CourseID = Assignment.CourseID;
            AllowedFileTypes = Assignment.AcceptedFileTypes; // Store allowed types for UI

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile uploadedFile)
        {
            int studentId = int.Parse(Request.Cookies["LoggedUserID"]);
            string filePath = "";

            // Handle Text Submissions
            if (Assignment.AcceptedFileTypes == "Text_Entry" && !string.IsNullOrWhiteSpace(TextSubmission))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", $"{Guid.NewGuid()}.txt");
                await System.IO.File.WriteAllTextAsync(filePath, TextSubmission);
            }

            // Handle File Submissions
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                string fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower().TrimStart('.');

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}.{fileExtension}");

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
            }

            try
            {
                CreateSubmission(AssignmentID, studentId, filePath.Replace(Directory.GetCurrentDirectory(), "").Replace("\\", "/"));

                await _context.SaveChangesAsync();
                return RedirectToPage("/Assignments/Index", new { id = CourseID });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Page();
            }
        }

        /// <summary>
        /// Verify then submit information to the database.
        /// </summary>
        /// <param name="assignmentID"></param>
        /// <param name="studentID"></param>
        /// <param name="filePath"></param>
        public void CreateSubmission(int assignmentID, int studentID, string filePath)
        {
            Submission submission = new Submission();

            if (assignmentID <= 0)
                throw new ArgumentOutOfRangeException("Parameter is out of range: InstructorID");
            else
            {
                Assignment? assignment = _context.Assignment.Where(c => c.Id == assignmentID).FirstOrDefault();
                if (assignment == null)
                    throw new ArgumentNullException("No Assignment Found.");

                submission.AssignmentID = assignmentID;
            }

            if (studentID <= 0)
                throw new ArgumentOutOfRangeException("Parameter is out of range: InstructorID");
            else
            {
                Models.Account? student = _context.Account.Where(c => c.Id == studentID).FirstOrDefault();
                if (student == null)
                    throw new ArgumentNullException("No Account Found.");

                submission.StudentID = studentID;
            }

            if (filePath == null || filePath == "")
                throw new ArgumentException("Argument cannot be null: No File Path Provided.");
            // TODO: Check if it actually pulls up a file.
            else submission.FilePath = filePath;

            _context.Submission.Add(submission);
        }
    }
}
