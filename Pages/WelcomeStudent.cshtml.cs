using CS3750Assignment1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS3750Assignment1.Pages {
    public class WelcomeStudentModel:PageModel {
        private readonly CS3750Assignment1Context _context;

        public WelcomeStudentModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int StudentId { get; set; }

        public void OnGet() {
            // Additional logic for student dashboard can be added here.
            StudentId = Int32.Parse(Request.Cookies["LoggedUserID"]);




            List<DayOfWeek> testlist = new List<DayOfWeek>() {DayOfWeek.Monday, DayOfWeek.Friday};
            string teststring = string.Join(",", testlist);
            Console.WriteLine(teststring);

        }
    }
}