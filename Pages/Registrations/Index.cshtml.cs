using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Registrations
{
    public class IndexModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public IndexModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IList<Registration> Registration { get; set; } = default!;

        int studentID;

        public async Task OnGetAsync()
        {
            studentID = int.Parse(Request.Cookies["LoggedUserID"]);

            // Fetch registrations only if ID is valid
            if (studentID > 0)
            {
                Registration = await _context.Registration.Where(c => c.StudentID == studentID).ToListAsync();

            }
            else
            {
                Registration = new List<Registration>(); // Avoid null reference issues
            }
        }
    }
}
