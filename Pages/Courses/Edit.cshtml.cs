using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Collections.Generic;

namespace CS3750Assignment1.Pages.Courses
{
    public class EditModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public EditModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; } = default!;

        [BindProperty]
        public bool MeetingSunday { get; set; }
        [BindProperty]
        public bool MeetingMonday { get; set; }
        [BindProperty]
        public bool MeetingTuesday { get; set; }
        [BindProperty]
        public bool MeetingWednesday { get; set; }
        [BindProperty]
        public bool MeetingThursday { get; set; }
        [BindProperty]
        public bool MeetingFriday { get; set; }
        [BindProperty]
        public bool MeetingSaturday { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Course = await _context.Course.FindAsync(id);
            if (Course == null)
            {
                return NotFound();
            }

            // Convert stored MeetingDays string into individual checkboxes
            var meetingDays = Course.MeetingDays?.Split(',') ?? new string[] { };

            MeetingSunday = meetingDays.Contains("Sunday");
            MeetingMonday = meetingDays.Contains("Monday");
            MeetingTuesday = meetingDays.Contains("Tuesday");
            MeetingWednesday = meetingDays.Contains("Wednesday");
            MeetingThursday = meetingDays.Contains("Thursday");
            MeetingFriday = meetingDays.Contains("Friday");
            MeetingSaturday = meetingDays.Contains("Saturday");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var courseToUpdate = await _context.Course.FindAsync(Course.Id);
            if (courseToUpdate == null)
            {
                return NotFound();
            }

            // Ensure the Instructor ID remains unchanged
            Course.InstructorID = courseToUpdate.InstructorID;

            // Update fields
            courseToUpdate.Name = Course.Name;
            courseToUpdate.CourseNumber = Course.CourseNumber;
            courseToUpdate.Credits = Course.Credits;
            courseToUpdate.Capacity = Course.Capacity;
            courseToUpdate.MeetingTime = Course.MeetingTime;
            courseToUpdate.Location = Course.Location;

            // Convert checkbox selections back to a string
            List<string> selectedMeetingDays = new List<string>();
            if (MeetingSunday) selectedMeetingDays.Add("Sunday");
            if (MeetingMonday) selectedMeetingDays.Add("Monday");
            if (MeetingTuesday) selectedMeetingDays.Add("Tuesday");
            if (MeetingWednesday) selectedMeetingDays.Add("Wednesday");
            if (MeetingThursday) selectedMeetingDays.Add("Thursday");
            if (MeetingFriday) selectedMeetingDays.Add("Friday");
            if (MeetingSaturday) selectedMeetingDays.Add("Saturday");

            courseToUpdate.MeetingDays = selectedMeetingDays.Count > 0 ? string.Join(",", selectedMeetingDays) : "None";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Course.Any(e => e.Id == Course.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/WelcomeInstructor");
        }

        // ✅ ADD THIS METHOD BELOW ONPostAsync
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var course = await _context.Course.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            _context.Course.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToPage("/WelcomeInstructor"); // Redirect back to instructor dashboard after deletion
        }
    }
}
