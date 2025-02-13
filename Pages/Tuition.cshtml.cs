using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Stripe;

namespace CS3750Assignment1.Pages {
    public class TuitionModel:PageModel {
        private readonly CS3750Assignment1Context _context;

        public TuitionModel(CS3750Assignment1Context context) {
            _context = context;
        }

        public IList<RegisteredCourseViewModel> RegisteredCourses { get; set; } = new List<RegisteredCourseViewModel>();
        public decimal TotalCost { get; set; }
        public bool IsPaymentMade { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public string PaymentStatus { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string paymentStatus) {
            var loggedUserId = Request.Cookies["LoggedUserID"];
            if (loggedUserId == null) {
                Console.WriteLine("LoggedUserID cookie is null. Redirecting to login page.");
                return RedirectToPage("/Index");
            }

            int studentID;
            if (!int.TryParse(loggedUserId,out studentID)) {
                Console.WriteLine("Invalid LoggedUserID cookie value. Redirecting to login page.");
                return RedirectToPage("/Index");
            }

            RegisteredCourses = await (from reg in _context.Registration
                                       join course in _context.Course
                                       on reg.CourseID equals course.Id
                                       where reg.StudentID == studentID
                                       select new RegisteredCourseViewModel {
                                           CourseName = course.Name,
                                           CourseNumber = course.CourseNumber.ToString(),
                                           Credits = course.Credits
                                       }).ToListAsync();

            TotalCost = RegisteredCourses.Sum(c => c.Credits * 300);

            if (paymentStatus == "success") {
                IsPaymentMade = true;
                TotalCost = 0; // Reset the total cost to 0 if payment is made
            }

            return Page();
        }

        public class RegisteredCourseViewModel {
            public string CourseName { get; set; }
            public string CourseNumber { get; set; }
            public int Credits { get; set; }
        }
    }
}