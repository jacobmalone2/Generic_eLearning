using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages {
    public class TuitionModel : PageModel {
        private readonly CS3750Assignment1Context _context;

        public TuitionModel(CS3750Assignment1Context context) {
            _context = context;
        }

        public IList<RegisteredCourseViewModel> RegisteredCourses { get; set; } = new List<RegisteredCourseViewModel>();
        public decimal TotalCost { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            if (Request.Cookies["LoggedUserID"] == null)
            {
                return RedirectToPage("/Index");
            }

            int studentID = int.Parse(Request.Cookies["LoggedUserID"]);

            RegisteredCourses = await (from reg in _context.Registration
                                       join course in _context.Course
                                       on reg.CourseID equals course.Id
                                       where reg.StudentID == studentID
                                       select new RegisteredCourseViewModel
                                       {
                                           CourseName = course.Name,
                                           CourseNumber = course.CourseNumber.ToString(),
                                           Credits = course.Credits
                                       }).ToListAsync();

            TotalCost = RegisteredCourses.Sum(c => c.Credits * 300);

            return Page();
        }

        public class RegisteredCourseViewModel {
            public string CourseName { get; set; }
            public string CourseNumber { get; set; }
            public int Credits { get; set; }
        }
    }
}
