using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS3750Assignment1.Pages
{
	public class WelcomeInstructorModel : PageModel
	{
		private readonly CS3750Assignment1Context _context;

		public WelcomeInstructorModel(CS3750Assignment1Context context)
		{
			_context = context;
		}

		public string FullName { get; set; } = string.Empty;

		public int InstructorId { get; set; } //

		public List<Course> InstructorCourses { get; set; } = new List<Course>();

		public bool? pageRole = true; // for calendar functionality

		public async Task<IActionResult> OnGetAsync()
		{
			// Try to get logged-in instructor ID from cookie
			if (!int.TryParse(Request.Cookies["LoggedUserID"], out int accountId))
			{
				return RedirectToPage("/Index");
			}

			InstructorId = accountId; 

			// Fetch instructor's full name
			var account = await _context.Account.FindAsync(accountId);
			if (account != null)
			{
				FullName = $"{account.FirstName} {account.LastName}";
			}

			// If new user has logged in, clear data from old user
			if (Request.Cookies["LastLoggedUserId"] != null)
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
                }
            }

			Response.Cookies.Append("LastLoggedUserId", accountId.ToString());

			// If cookies courses null, load courses from database and save to cookies
			if (Request.Cookies["SavedCourseIds"] == null) 
			{
                // Get courses taught by the instructor from the database
                InstructorCourses = await _context.Course
                    .Where(c => c.InstructorID == InstructorId)
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

				foreach (Course c in InstructorCourses)
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

					InstructorCourses.Add(newCourse);
				}

            }
				
			return Page();
		}
	}
}
