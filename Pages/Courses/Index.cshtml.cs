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

        public async Task OnGetAsync(int id)
        {
            // Recieve Instructor ID
            instructorID = id;

            // </snippet_search_linqQuery>
            var courses = from c in _context.Course
                            select c;

            courses = courses.Where(c => c.InstructorID == id);
            Course = await courses.ToListAsync();
        }
    }
}
