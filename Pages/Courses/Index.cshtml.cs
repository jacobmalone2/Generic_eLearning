using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public IndexModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; } = default!;

        public int instructorID { get; set; }

        public async Task OnGetAsync(int? id) {
            if (id == null) {
                instructorID = 0; // Default value if no ID is passed
            }
            else {
                instructorID = id.Value;
            }

            // Fetch courses only if ID is valid
            if (instructorID > 0) {
                Course = await _context.Course.Where(c => c.InstructorID == instructorID).ToListAsync();
            }
            else {
                Course = new List<Course>(); // Avoid null reference issues
            }
        }
    }
}
