using CS3750Assignment1.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using NuGet.Protocol;


namespace CS3750Assignment1.Pages
{
    public class IndexModel : PageModel
    {
        //private readonly ILogger<IndexModel> _logger;

        private readonly CS3750Assignment1.Data.CS3750Assignment1Context? _context;

        public IndexModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public string username { get; set; }

        [BindProperty]
        public string password { get; set; }



        public void OnGet()
        {

        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            
            //await _context.SaveChangesAsync();
            int desiredUserID = _context.Account
                .Where(a => a.Username == username && a.Password == password)
                .Select(a => a.Id)
                .FirstOrDefault();
            //from acc in _context.Account where Username == Account.Username AND Account.Password == Account.Password;

            Console.WriteLine(desiredUserID);
            /*var userAccount = await _context.Account.FindAsync(desiredUserID);

            if (userAccount == null)
            {
                return NotFound();
            }*/

            return RedirectToPage("./WelcomeUser/" + desiredUserID + "?");
        }
    }
}
