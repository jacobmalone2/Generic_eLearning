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
    public class CalendarInstructorModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public bool? pageRole = true; //null means no role, false means student, true means instructor

        public CalendarInstructorModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; } = default!;

        public async Task OnGetAsync()
        {
            //The following code was commented out on Feb 7, 2025 to get the calendar loaded on the page but the calendar does not yet have instructor course functionality
            //Course = await _context.Course
              //  .Include(c => c.Account).ToListAsync();
        }
    }
}
