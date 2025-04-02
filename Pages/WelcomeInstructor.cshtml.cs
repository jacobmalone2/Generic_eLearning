using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS3750Assignment1.Pages
{
	public class WelcomeInstructorModel : PageModel
	{
		private readonly CS3750Assignment1Context _context;

		public WelcomeInstructorModel(CS3750Assignment1Context context)
		{
			_context = context;
		}

		public string FullName { get; set; } = string.Empty;

		public int InstructorId { get; set; } //

		public List<Course> InstructorCourses { get; set; } = new List<Course>();

		public bool? pageRole = true; // for calendar functionality

		public async Task<IActionResult> OnGetAsync()
		{
			// Try to get logged-in instructor ID from cookie
			if (!int.TryParse(Request.Cookies["LoggedUserID"], out int accountId))
			{
				return RedirectToPage("/Index");
			}

			InstructorId = accountId; 

			// Fetch instructor's full name
			var account = await _context.Account.FindAsync(accountId);
			if (account != null)
			{
				FullName = $"{account.FirstName} {account.LastName}";
			}

			// Get courses taught by the instructor
			InstructorCourses = await _context.Course
				.Where(c => c.InstructorID == InstructorId)
				.ToListAsync();

			return Page();
		}
	}
}
