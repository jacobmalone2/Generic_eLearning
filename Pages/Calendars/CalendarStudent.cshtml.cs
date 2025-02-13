using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Calendars
{
    public class CalendarStudentModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public bool? pageRole = false;

        public CalendarStudentModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IList<Registration> Registration { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            //the following code block will likely be used later when the calendar is integrated with the student's course information
            //as of Feb 7, 2025 this has been commented out to show that the Calendar itself can be loaded and used on the webpage but
            //the calendar does not work with course information yet
            //Registration = await _context.Registration
            //    .Include(r => r.Account)
            //    .Include(r => r.Course).ToListAsync();
            return Page();
        }
    }
}
