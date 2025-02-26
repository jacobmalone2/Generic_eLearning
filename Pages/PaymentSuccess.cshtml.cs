using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750Assignment1.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CS3750Assignment1.Pages {
    // this page loads when the payment was successful. 
    // the only logic that this page needs is to update the IsPaid field in the Registration table to 
    // true for the courses that the student has paid for.
    public class PaymentSuccessModel:PageModel {
        private readonly CS3750Assignment1Context _context;
        public PaymentSuccessModel(CS3750Assignment1Context context) {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync() {
            int studentID = int.Parse(Request.Cookies["LoggedUserID"]);
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

            //RedirectToAction("Tuition");
            return Redirect("/Tuition");
        }

    }

}

    