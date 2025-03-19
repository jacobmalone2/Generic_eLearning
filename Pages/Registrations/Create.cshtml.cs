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
using Stripe;

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

            try
            {
                CreateRegistration(Id, studentID); // Id variable is the Course ID being submitted.
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Page();
            }

            return RedirectToPage("/WelcomeStudent");
        }

        /// <summary>
        /// Verifies then submits information to the database.
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="studentID"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void CreateRegistration(int courseID, int studentID)
        {
            Registration registration = new Registration();
            registration.Id = 0; // Unless you feel like going down a rabit hole, don't delete this line.

            if (courseID <= 0)
                throw new ArgumentOutOfRangeException("Parameter is out of range: CourseID");
            else
            {
                Course? Course = _context.Course.Where(c => c.Id == courseID).FirstOrDefault();
                if (Course == null)
                    throw new ArgumentNullException("No Course Found.");

                registration.CourseID = courseID;
            }

            if (studentID <= 0)
                throw new ArgumentOutOfRangeException("Parameter is out of range: StudentID");
            else
            {
                Models.Account? Student = _context.Account.Where(c => c.Id == studentID).FirstOrDefault();
                if (Student == null)
                    throw new ArgumentNullException("No Account Found.");

                registration.StudentID = studentID;
            }

            _context.Registration.Add(registration);
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
