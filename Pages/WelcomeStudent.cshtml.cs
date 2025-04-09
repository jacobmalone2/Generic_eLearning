using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CS3750Assignment1.Pages
{

    public class WelcomeStudentModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public WelcomeStudentModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]

        public string FullName { get; set; } = string.Empty;

        public int StudentId { get; set; }

        public bool? pageRole = false; // Added for calendar functionality

        public List<Course> RegisteredCourses { get; set; } = new List<Course>();

        public List<Assignment> Assignments { get; set; } = new List<Assignment>(); // Temporary list to hold assignments for each course
        public record AssignmentWithCourse(Assignment Assignment, Course Course); // Structure that holds assignments and courses together
        public List<AssignmentWithCourse> CourseAssignments { get; set; } = new List<AssignmentWithCourse>(); // List of all assignments with their corresponding courses

        public async Task<IActionResult> OnGetAsync()
        {
            // Get Student ID from cookie
            if (!int.TryParse(Request.Cookies["LoggedUserID"], out int accountId))
            {
                return RedirectToPage("/Index");
            }

			StudentId = accountId;


			// Fetch student's name
			var account = await _context.Account.FindAsync(accountId);
            if (account != null)
            {
                FullName = $"{account.FirstName} {account.LastName}";
            }

            // True if courses already exist
            bool savedCoursesExist = !Request.Cookies["SavedCourseIds"].IsNullOrEmpty();

            // If new user has logged in, clear data from old user
            if (!Request.Cookies["LastLoggedUserId"].IsNullOrEmpty())
            {
                if (int.Parse(Request.Cookies["LastLoggedUserId"]) != accountId)
                {
                    Response.Cookies.Delete("SavedCourseIds");
                    Response.Cookies.Delete("SavedInstructorIds");
                    Response.Cookies.Delete("SavedCourseNames");
                    Response.Cookies.Delete("SavedCourseNumbers");
                    Response.Cookies.Delete("SavedCourseCredits");
                    Response.Cookies.Delete("SavedCourseCapacities");
                    Response.Cookies.Delete("SavedCourseMeetingDays");
                    Response.Cookies.Delete("SavedCourseMeetingTimes");
                    Response.Cookies.Delete("SavedCourseLocations");
                    Response.Cookies.Delete("SavedCourseDepartments");

                    savedCoursesExist = false; // Saved courses were deleted, so set to false
                }
            }

            //Response.Cookies.Append("LastLoggedUserId", accountId.ToString());

            // If cookies courses null, load courses from database and save to cookies
            if (!savedCoursesExist)
            {
                // Fetch the courses that the student is registered for
                RegisteredCourses = await _context.Registration
                    .Where(r => r.StudentID == StudentId)
                    .Include(r => r.Course) // Load course details
                    .Select(r => r.Course)
                    .ToListAsync();


                // Create list for every parameter of each course
                List<int> CourseIds = new List<int>();
                List<int> InstructorIds = new List<int>();
                List<string> CourseNames = new List<string>();
                List<int> CourseNumbers = new List<int>();
                List<int> CourseCredits = new List<int>();
                List<int> CourseCapacities = new List<int>();
                List<string> CourseMeetingDays = new List<string>();
                List<string> CourseMeetingTimes = new List<string>();
                List<string> CourseLocations = new List<string>();
                List<string> CourseDepartments = new List<string>();

                foreach (Course c in RegisteredCourses)
                {
                    CourseIds.Add(c.Id);
                    InstructorIds.Add(c.InstructorID);
                    CourseNames.Add(c.Name);
                    CourseNumbers.Add(c.CourseNumber);
                    CourseCredits.Add(c.Credits);
                    CourseCapacities.Add(c.Capacity);
                    CourseMeetingDays.Add(c.MeetingDays);
                    CourseMeetingTimes.Add(c.MeetingTime);
                    CourseLocations.Add(c.Location);
                    CourseDepartments.Add(c.Department);
                }

                // Save lists as strings
                string listString = string.Join(",", CourseIds);
                Response.Cookies.Append("SavedCourseIds", listString);
                listString = string.Join(",", InstructorIds);
                Response.Cookies.Append("SavedInstructorIds", listString);
                listString = string.Join(",", CourseNames);
                Response.Cookies.Append("SavedCourseNames", listString);
                listString = string.Join(",", CourseNumbers);
                Response.Cookies.Append("SavedCourseNumbers", listString);
                listString = string.Join(",", CourseCredits);
                Response.Cookies.Append("SavedCourseCredits", listString);
                listString = string.Join(",", CourseCapacities);
                Response.Cookies.Append("SavedCourseCapacities", listString);
                listString = string.Join(",", CourseMeetingDays);
                Response.Cookies.Append("SavedCourseMeetingDays", listString);
                listString = string.Join(",", CourseMeetingTimes);
                Response.Cookies.Append("SavedCourseMeetingTimes", listString);
                listString = string.Join(",", CourseLocations);
                Response.Cookies.Append("SavedCourseLocations", listString);
                listString = string.Join(",", CourseDepartments);
                Response.Cookies.Append("SavedCourseDepartments", listString);

                Response.Cookies.Append("LastLoggedUserId", accountId.ToString());


            }
            // Else, load courses from cookies
            else
            {
                // Parse lists from strings saved in cookies
                List<int> CourseIds = new List<int>();
                List<int> InstructorIds = new List<int>();
                List<string> CourseNames = new List<string>();
                List<int> CourseNumbers = new List<int>();
                List<int> CourseCredits = new List<int>();
                List<int> CourseCapacities = new List<int>();
                List<string> CourseMeetingDays = new List<string>();
                List<string> CourseMeetingTimes = new List<string>();
                List<string> CourseLocations = new List<string>();
                List<string> CourseDepartments = new List<string>();

                string listString = Request.Cookies["SavedCourseIds"];
                CourseIds = listString.Split(',').Select(int.Parse).ToList();
                listString = Request.Cookies["SavedInstructorIds"];
                InstructorIds = listString.Split(',').Select(int.Parse).ToList();
                listString = Request.Cookies["SavedCourseNames"];
                CourseNames = listString.Split(',').ToList();
                listString = Request.Cookies["SavedCourseNumbers"];
                CourseNumbers = listString.Split(',').Select(int.Parse).ToList();
                listString = Request.Cookies["SavedCourseCredits"];
                CourseCredits = listString.Split(',').Select(int.Parse).ToList();
                listString = Request.Cookies["SavedCourseCapacities"];
                CourseCapacities = listString.Split(',').Select(int.Parse).ToList();
                listString = Request.Cookies["SavedCourseMeetingDays"];
                CourseMeetingDays = listString.Split(',').ToList();
                listString = Request.Cookies["SavedCourseMeetingTimes"];
                CourseMeetingTimes = listString.Split(',').ToList();
                listString = Request.Cookies["SavedCourseLocations"];
                CourseLocations = listString.Split(',').ToList();
                listString = Request.Cookies["SavedCourseDepartments"];
                CourseDepartments = listString.Split(',').ToList();

                int size = CourseIds.Count();
                for (int i = 0; i < size; i++)
                {
                    Course newCourse = new Course();
                    newCourse.Id = CourseIds[i];
                    newCourse.InstructorID = InstructorIds[i];
                    newCourse.Name = CourseNames[i];
                    newCourse.CourseNumber = CourseNumbers[i];
                    newCourse.Credits = CourseCredits[i];
                    newCourse.Capacity = CourseCapacities[i];
                    newCourse.MeetingDays = CourseMeetingDays[i];
                    newCourse.MeetingTime = CourseMeetingTimes[i];
                    newCourse.Location = CourseLocations[i];
                    newCourse.Department = CourseDepartments[i];

                    RegisteredCourses.Add(newCourse);
                }

            }

            // Fetch the assignments for each course
            foreach (Course c in RegisteredCourses)
            {
                Assignments = await _context.Assignment //get all assignments for course
                    .Where(a => a.CourseID == c.Id)
                    //.Where(a => DateTime.ParseExact(a.DueDate, "yyyy-mm-dd", CultureInfo.InvariantCulture) > DateTime.Now)
                    //.Include(a => a.Assignment)
                    //.Select(a => a.Assignment)
                    .ToListAsync();

                foreach (Assignment a in Assignments) // add assignments to total list
                {
                    //Added this try/catch block to let me log in as teststudent when testing student assignment graph
                    // was crashing with some strange 2100/05/05 date not being recognized as a datetime object error
                    // 3/25/2025 11:15pm -Carter
                    try
                    {
                        // filter out past assignments
                        if (DateTime.ParseExact(a.DueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) < DateTime.Now) {
                            continue;
                        }
                        AssignmentWithCourse w = new AssignmentWithCourse(a, c);
                        CourseAssignments.Add(w);
                    }
                    catch
                    { continue; }
                } 
            }
            // Load notifications
            var notifications = await _context.Notification
                .Where(n => n.AccountId == StudentId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .ToListAsync();

            ViewData["Notifications"] = notifications;
            ViewData["HasUnseenNotifications"] = notifications.Any(n => !n.IsSeen);


            // Sort the list based on due date
            CourseAssignments.Sort((a1, a2) => DateTime.ParseExact(a1.Assignment.DueDate + " " + a1.Assignment.DueTime, "yyyy-MM-dd h:mmtt", CultureInfo.InvariantCulture).CompareTo(DateTime.ParseExact(a2.Assignment.DueDate + " " + a2.Assignment.DueTime, "yyyy-MM-dd h:mmtt", CultureInfo.InvariantCulture)));
            

            return Page();


        }
    }

}
