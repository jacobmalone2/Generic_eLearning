using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Registrations
{
    public class IndexModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public IndexModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        // Use a ViewModel instead of Registration directly
        public IList<RegistrationViewModel> Registrations { get; set; } = new List<RegistrationViewModel>();

        int studentID;

        public async Task OnGetAsync()
        {
            studentID = int.Parse(Request.Cookies["LoggedUserID"]);

            if (studentID > 0)
            {
                // LINQ JOIN to include Course Name
                Registrations = await (from reg in _context.Registration
                                       join course in _context.Course
                                       on reg.CourseID equals course.Id
                                       where reg.StudentID == studentID
                                       select new RegistrationViewModel
                                       {
                                           Id = reg.Id,
                                           StudentID = reg.StudentID,
                                           CourseID = reg.CourseID,
                                           CourseName = course.Name, // Added Course Name
                                           CourseNumber = course.CourseNumber,
                                           Capacity = course.Capacity,
                                           Credits = course.Credits,
                                           MeetingDays = course.MeetingDays,
                                           MeetingTime = course.MeetingTime,
                                           Location = course.Location
                                       }).ToListAsync();
            }
        }

        // Custom ViewModel to hold Registration and Course details
        public class RegistrationViewModel
        {
            // New fields for Course information
            public int Id { get; set; }
            public int StudentID { get; set; }
            public int CourseID { get; set; }
            public string CourseName { get; set; }
            public int CourseNumber { get; set; }
            public int Capacity{ get; set; }
            public int Credits { get; set; }
            public string MeetingDays { get; set; }
            public string MeetingTime { get; set; }
            public string Location { get; set; }
        }
    }
}
