using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Stripe.Checkout;
using System.Diagnostics;

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

            if (!int.TryParse(loggedUserId,out int studentID)) {
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
                                           Credits = course.Credits,
                                           Department = course.Department,
                                           IsPaid = reg.IsPaid
                                       }).ToListAsync();
            // Calculate total cost
            
            IsPaymentMade = RegisteredCourses.All(c => c.IsPaid);

            // check to see if the courses are paid for
            if (IsPaymentMade) {
                TotalCost = 0;
            }
            else {
                TotalCost = RegisteredCourses.Sum(c => c.Credits * 300);
            }

            if (!string.IsNullOrEmpty(paymentStatus)) {
                Debug.WriteLine(paymentStatus);
                if (paymentStatus == "success") {
                    Debug.WriteLine($"Payment status: {paymentStatus}");
                    // Update the registration table to show that the student has paid for the courses
                    foreach (var course in RegisteredCourses) {
                        var registration = await _context.Registration.Where(r => r.StudentID == studentID && r.CourseID == int.Parse(course.CourseNumber)).ToListAsync();//FirstOrDefaultAsync(r => r.StudentID == studentID && r.CourseID == int.Parse(course.CourseNumber));
                        if (registration != null && registration.Count == 1) {
                            registration[0].IsPaid = true;
                            _context.Registration.Update(registration[0]);
                        }
                    }
                    await _context.SaveChangesAsync();
                    IsPaymentMade = true;
                }
                else if (paymentStatus == "cancel") {
                    // Do nothing
                }
            }


            return Page();
        }

       
        public IActionResult OnPost() {
            var options = new SessionCreateOptions {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = (long)(TotalCost * 300), 
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Tuition Payment",
                            },
                        },
                        Quantity = 1,
                    }
                },
                Mode = "payment",
                SuccessUrl = "/Tuition?paymentStatus=success",
                CancelUrl = "/Tuition?paymentStatus=cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }
        

        public class RegisteredCourseViewModel {
            public string CourseName { get; set; }
            public string CourseNumber { get; set; }
            public int Credits { get; set; }
            public string Department { get; set; }
            public bool IsPaid { get; set; }
        }
    }
}
