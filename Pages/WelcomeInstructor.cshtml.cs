using CS3750Assignment1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS3750Assignment1.Pages {
    public class WelcomeInstructorModel:PageModel {
        private readonly CS3750Assignment1Context _context;

        public WelcomeInstructorModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int InstructorId { get; set; }

        public bool? pageRole = true; //added for calendar functionality

        public void OnGet() {
            // Additional logic for instructor dashboard can be added here.
            InstructorId = Int32.Parse(Request.Cookies["LoggedUserID"]);

        }
    }
}
