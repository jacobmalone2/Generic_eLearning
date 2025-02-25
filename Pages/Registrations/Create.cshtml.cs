using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.EntityFrameworkCore;

namespace CS3750Assignment1.Pages.Registrations
{
    public class CreateModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public CreateModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Registration Registration { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? Departments { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CourseDepartment { get; set; }

        public IList<Course> Courses { get; set; } = default!;
        public IList<Registration> Registrations { get; set; } = default!;

        int studentID;

        public async Task OnGetAsync()
        {
            studentID = int.Parse(Request.Cookies["LoggedUserID"]);

            // <snippet_search_linqQuery>
            IQueryable<string> departmentQuery = from d in _context.Course
                                            orderby d.Department
                                            select d.Department;
            // <snippet_search_selectList>
            Departments = new SelectList(await departmentQuery.Distinct().ToListAsync());

            try
            {
                var CourseList = from c in _context.Course select c;

                if (!string.IsNullOrEmpty(SearchString))
                {
                    CourseList = CourseList.Where(c => c.Name.Contains(SearchString));
                }
                if (!string.IsNullOrEmpty(CourseDepartment))
                {
                    CourseList = CourseList.Where(c => c.Department == CourseDepartment);
                }

                Courses = await CourseList.ToListAsync();
            }
            catch (Exception ex)
            {
                Courses = new List<Course>(); // Avoid null reference issues
                Console.WriteLine("No Courses Found: " + ex);
            }

            try
            {
                Registrations = await _context.Registration.ToListAsync();
            }
            catch (Exception ex)
            {
                Registrations = new List<Registration>(); // Avoid null reference issues
                Console.WriteLine("No Registrations Found: " + ex);
            }
        }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int Id)
        {
            if (!ModelState.IsValid || Request.Cookies["LoggedUserRole"] != "Student")
            {
                return Page();
            }

            studentID = int.Parse(Request.Cookies["LoggedUserID"]);

            Registration.StudentID = studentID;
            Registration.CourseID = Id; // Fetch Course ID from register button.

            Registration.Id = 0; // Unless you feel like going down a rabit hole, don't delete this line.

            _context.Registration.Add(Registration);
            await _context.SaveChangesAsync();

            return RedirectToPage("/WelcomeStudent");
        }

        public bool IsRegistered(int x)
        {
            foreach (Registration r in Registrations)
            {
                if (r.CourseID == x && r.StudentID == int.Parse(Request.Cookies["LoggedUserID"])) return true;
            }

            return false;
        }
    }
}
