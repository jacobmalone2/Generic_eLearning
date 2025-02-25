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

            Assignment = await _context.Assignment
                .Where(a => a.Id == AssignmentID)
                .Select(a => new Assignment
                {
                    Id = a.Id,
                    CourseID = a.CourseID,
                    AcceptedFileTypes = a.AcceptedFileTypes
                })
                .FirstOrDefaultAsync();

            if (Assignment == null)
                return NotFound();

            CourseID = Assignment.CourseID;
            AllowedFileTypes = Assignment.AcceptedFileTypes; // Store allowed types for UI

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile uploadedFile)
        {
            int studentId = int.Parse(Request.Cookies["LoggedUserID"]);

            // Fetch Assignment
            Assignment = await _context.Assignment.FindAsync(AssignmentID);
            if (Assignment == null)
                return NotFound();

            // Get allowed file types
            string[] allowedTypes = Assignment.AcceptedFileTypes.Split(',');

            // Handle Text Submissions
            if (allowedTypes.Contains("txt") && !string.IsNullOrWhiteSpace(TextSubmission))
            {
                string textFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", $"{Guid.NewGuid()}.txt");
                await System.IO.File.WriteAllTextAsync(textFilePath, TextSubmission);

                Submission = new Submission
                {
                    AssignmentID = AssignmentID,
                    StudentID = studentId,
                    FilePath = textFilePath.Replace(Directory.GetCurrentDirectory(), "").Replace("\\", "/"),
                };

                _context.Submission.Add(Submission);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Assignments/Index", new { id = CourseID });
            }

            // Handle PDF Submissions
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                string fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower().TrimStart('.');
                if (!allowedTypes.Contains(fileExtension))
                {
                    ModelState.AddModelError("", $"Invalid file type. Allowed types: {Assignment.AcceptedFileTypes}");
                    return Page();
                }

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}.{fileExtension}");

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                Submission = new Submission
                {
                    AssignmentID = AssignmentID,
                    StudentID = studentId,
                    FilePath = filePath.Replace(Directory.GetCurrentDirectory(), "").Replace("\\", "/"),
                };

                _context.Submission.Add(Submission);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Assignments/Index", new { id = CourseID });
            }

            ModelState.AddModelError("", "No valid submission found. Please enter text or upload a file.");
            return Page();
        }
    }
}
