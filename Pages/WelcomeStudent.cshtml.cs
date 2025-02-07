using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS3750Assignment1.Pages {
    public class WelcomeStudentModel:PageModel {
        [BindProperty(SupportsGet = true)]
        public int StudentId { get; set; }

        public bool? pageRole = false; //added for calendar functionality

        public void OnGet() {
            // Additional logic for student dashboard can be added here.
        }
    }
}