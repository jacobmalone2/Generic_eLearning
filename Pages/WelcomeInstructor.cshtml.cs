using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS3750Assignment1.Pages {
    public class WelcomeInstructorModel:PageModel {
        [BindProperty(SupportsGet = true)]
        public int InstructorId { get; set; }

        public bool? pageRole = true; //added for calendar functionality

        public void OnGet() {
            // Additional logic for instructor dashboard can be added here.
        }
    }
}
