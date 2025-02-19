using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Courses {
    public class CreateModel : PageModel {
        private readonly CS3750Assignment1Context _context;

        [BindProperty]
        public Course Course { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int instructorID { get; set; }  // Receive the Instructor ID

        //I hope there is a cleaner way of doing this in the future but for now this *should* do just fine
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
        //meeting day properties

        [BindProperty]
        public string MeetingTimeStart { get; set; }

        [BindProperty]
        public string MeetingTimeEnd { get; set; }

        [BindProperty]
        public string Department { get; set; }

        public CreateModel(CS3750Assignment1Context context) {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync() {
            instructorID = int.Parse(Request.Cookies["LoggedUserID"]);
            Course.InstructorID = instructorID;  // Assign instructor ID to new course

            //check what days have been submitted and create a string with each meeting day
            //each day will be separated by a comma and no space in between, i.e: Sunday,Monday,Thursday,Saturday
            //each string will start with the earliest day in the week, the earliest day being Sunday and the latest day being Saturday
            string meetingDayString = "";
            List<string> selectedMeetingDays = new List<string>();

            //gather the selected meeting days
            if (MeetingSunday)
            {
                selectedMeetingDays.Add("Sunday");
            }
            if (MeetingMonday)
            {
                selectedMeetingDays.Add("Monday");
            }
            if (MeetingTuesday)
            {
                selectedMeetingDays.Add("Tuesday");
            }
            if (MeetingWednesday)
            {
                selectedMeetingDays.Add("Wednesday");
            }
            if (MeetingThursday)
            {
                selectedMeetingDays.Add("Thursday");
            }
            if (MeetingFriday)
            {
                selectedMeetingDays.Add("Friday");
            }
            if (MeetingSaturday)
            {
                selectedMeetingDays.Add("Saturday");
            }

            //make sure there was at least one day selected
            if (selectedMeetingDays.Count > 0)
            {
                meetingDayString = string.Join(",", selectedMeetingDays); //very proud of this
            }
            //if there was no day selected, the string will be 'None'
            else
            {
                meetingDayString = "None";
            }

            //set the course meeting days to that created string
            Course.MeetingDays = meetingDayString; //set the MeetingDays value to the meeting day string created from the given meeting days
            Course.MeetingTime = String.Concat(MeetingTimeStart, "-", MeetingTimeEnd);
            Course.Department = Department;

            if (!ModelState.IsValid || Request.Cookies["LoggedUserRole"] != "Instructor")
            {
                return Page();
            }

            _context.Course.Add(Course);
            await _context.SaveChangesAsync();

            return RedirectToPage("/WelcomeInstructor");
        }
    }
}
