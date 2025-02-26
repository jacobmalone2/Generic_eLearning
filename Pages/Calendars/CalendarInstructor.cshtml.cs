using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Calendars
{
    public class CalendarInstructorModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public bool? pageRole = true; //null means no role, false means student, true means instructor

        public CalendarInstructorModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; } = default!;

        public IList<RegisteredCourse> InstructorCourses { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int loggedUserID;
            InstructorCourses = [];
            if (Request.Cookies["LoggedUserID"] is not null)
            {
                try
                {
                    loggedUserID = int.Parse(Request.Cookies["LoggedUserID"]);
                }
                catch
                {
                    loggedUserID = 0;
                }

            }
            else
            {
                loggedUserID = 0;
            }

            
            Course = await _context.Course.Where(c => c.InstructorID == loggedUserID).ToListAsync();
            //.Include(c => c.Account).ToListAsync();

            foreach (var course in Course)
            {
                if (course.MeetingDays is not null && course.MeetingTime is not null)
                {
                    List<string> classDays = new List<string>();
                    string classStart, classEnd, extra;
                    foreach (string day in course.MeetingDays.Split(','))
                    {
                        classDays.Add(day);
                        //will need to change this. Full Calendar wants the days of the week in number form: 0 for Sunday, 1 for Monday, 2 for Tuesday, etc
                    }
                    string[] times = course.MeetingTime.Split("-");

                    RegisteredCourse tempCourse = new RegisteredCourse(course.Name, course.CourseNumber, classDays, times[0], times[1]);

                    InstructorCourses.Add(tempCourse);

                }



            }

            return Page();
        }
    }
}
