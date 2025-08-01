using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750Assignment1.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CS3750Assignment1.Pages {
    // this page loads when the payment was successful. 
    // the only logic that this page needs is to update the IsPaid field in the Registration table to 
    // true for the courses that the student has paid for.
    public class SuccessModel:PageModel {
        private readonly CS3750Assignment1Context _context;
        public SuccessModel(CS3750Assignment1Context context) {
            _context = context;
        }

        public string LoggedUserID { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            Console.WriteLine("made it to get");
            // Retrieve the cookie value
            //the key to this working properly is the userID cookie has cross-site functionality. I think this could potentially be a vulnerability but I'm not sure of our options other than the secure setting on the cookie
            if (Request.Cookies.TryGetValue("LoggedUserID", out var userId))
            {
                LoggedUserID = userId;
            }
            else
            {
                LoggedUserID = "Cookie not found";
            }

            return Page();
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostProcessPaymentAsync()
        {
            LoggedUserID = Request.Cookies["LoggedUserID"];
            int studentID = int.Parse(LoggedUserID);
            Console.WriteLine("Student ID is: " + studentID);
            var registrations = await _context.Registration
                .Where(r => r.StudentID == studentID && !r.IsPaid)
                .ToListAsync();

            foreach (var registration in registrations)
            {
                registration.IsPaid = true;
                _context.Registration.Update(registration);
            }

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
    }

}

    