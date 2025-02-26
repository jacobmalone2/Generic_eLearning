using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750Assignment1.Data;
using Microsoft.EntityFrameworkCore;

namespace CS3750Assignment1.Pages {
    // this page loads when the payment was successful. 
    // the only logic that this page needs is to update the IsPaid field in the Registration table to 
    // true for the courses that the student has paid for.
    public class SuccessModel:PageModel {
        private readonly CS3750Assignment1Context _context;
        public SuccessModel(CS3750Assignment1Context context) {
            _context = context;
        }

        public async Task OnGetAsync() {
            var loggedUserId = Request.Cookies["LoggedUserID"];
            if (loggedUserId == null) {
                Console.WriteLine("LoggedUserID cookie is null. Redirecting to login page.");
                RedirectToPage("/Index");
                return;
            }

            if (!int.TryParse(loggedUserId,out int studentID)) {
                Console.WriteLine("Invalid LoggedUserID cookie value. Redirecting to login page.");
                RedirectToPage("/Index");
                return;
            }

            var registrations = await _context.Registration
                .Where(r => r.StudentID == studentID && !r.IsPaid)
                .ToListAsync();

            foreach (var registration in registrations) {
                registration.IsPaid = true;
                _context.Registration.Update(registration);
            }

            await _context.SaveChangesAsync();
        }
    }
}