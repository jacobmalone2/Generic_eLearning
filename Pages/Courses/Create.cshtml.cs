using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Globalization;

namespace CS3750Assignment1.Pages.Courses {
    public class CreateModel : PageModel 
    {
        private readonly CS3750Assignment1Context _context;

        [BindProperty]
        public Course UserCourse { get; set; } = default!;

        // Meeting days properties
        // I hope there is a cleaner way of doing this in the future but for now this *should* do just fine
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

        // Meeting Times
        [BindProperty]
        public string MeetingTimeStart { get; set; }

        [BindProperty]
        public string MeetingTimeEnd { get; set; }

        // Html code breaks if we don't do this for some reason.
        [BindProperty]
        public string Department { get; set; }

        public CreateModel(CS3750Assignment1Context context) {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid || Request.Cookies["LoggedUserRole"] != "Instructor")
            {
                return Page();
            }

            int instructorID = int.Parse(Request.Cookies["LoggedUserID"]); // Receive the Instructor ID

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
            
            // Grab page information, and create course.
            CreateCourse(instructorID, UserCourse.Name, UserCourse.CourseNumber, Department, UserCourse.Location
                , UserCourse.Credits, UserCourse.Capacity, meetingDayString, MeetingTimeStart, MeetingTimeEnd);
            await _context.SaveChangesAsync();

            return RedirectToPage("/WelcomeInstructor");
        }

        /// <summary>
        /// Used to verify then submit course information to the database.
        /// </summary>
        /// <param name="instructorID"></param>
        /// <param name="name"></param>
        /// <param name="courseNumber"></param>
        /// <param name="department"></param>
        /// <param name="location"></param>
        /// <param name="credits"></param>
        /// <param name="capacity"></param>
        /// <param name="meetingDays"></param>
        /// <param name="meetingTimeStart"></param>
        /// <param name="meetingTimeEnd"></param>
        /// <returns></returns>
        public void CreateCourse(int instructorID, string name, int courseNumber, string department, string location, int credits, int capacity, string meetingDays, string meetingTimeStart, string meetingTimeEnd)
        {
            Course course = new Course();

            if (instructorID <= 0)
                throw new ArgumentOutOfRangeException("Parameter is out of range: InstructorID");
            else
            {
                Account? Instructor = _context.Account.Where(c => c.Id == instructorID).FirstOrDefault();
                if (Instructor == null)
                    throw new ArgumentNullException("No Account Found.");

                course.InstructorID = instructorID;
            }

            if (name == null || name == "")
                throw new ArgumentException("Argument cannot be null: No Course Name Provided.");
            else course.Name = name;

            if (courseNumber == 0)
                throw new ArgumentException("Argument cannot be null: No Course Number Provided.");
            else course.CourseNumber = courseNumber;

            if (department == null || department == "")
                throw new ArgumentException("Argument cannot be null: No Course Department Provided.");
            else course.Department = department;

            if (location == null || location == "")
                throw new ArgumentException("Argument cannot be null: No Course Location Provided.");
            else if (location.Length > 30 || location.Length < 2)
                throw new ArgumentOutOfRangeException("Location must be between two to thirty characters.");
            else course.Location = location;

            if (credits < 1)
                throw new ArgumentException("Parameter is out of range: Credits Cannot Be Negative or Zero.");
            else course.Credits = credits;

            if (capacity < 1 || capacity > 200)
                throw new ArgumentException("Parameter is out of range: Capacity Must Be Between One to Two-Hundred.");
            else course.Capacity = capacity;

            string[] selectedMeetingDays = meetingDays.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in selectedMeetingDays)
            {
                if (!Enum.IsDefined(typeof(DayOfWeek), s) && s != "None")
                    throw new ArgumentException("Argument Must Be A Day Of The Week, Or None.");
            }
            course.MeetingDays = meetingDays;

            DateTime temp = new DateTime();
            if (!DateTime.TryParseExact(meetingTimeStart, "hh:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out temp))
                throw new ArgumentException("Incorrect Datetime Format. Example: 10:30am");
            else
            {
                if (!DateTime.TryParseExact(meetingTimeEnd, "hh:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out temp))
                    throw new ArgumentException("Incorrect Datetime Format. Example: 10:30am");
                else
                    course.MeetingTime = String.Concat(meetingTimeStart, "-", meetingTimeEnd);
            }

            _context.Course.Add(course);
        }
    }
}
