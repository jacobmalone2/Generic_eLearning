
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750Assignment1.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CS3750Assignment1.Pages
{
    public class notificationSeenModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public notificationSeenModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!int.TryParse(Request.Cookies["LoggedUserID"], out int studentId))
            {
                return new JsonResult(new { success = false });
            }

            var unseen = _context.Notification.Where(n => n.AccountId == studentId && !n.IsSeen);
            foreach (var note in unseen)
            {
                note.IsSeen = true;
            }

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
    }
}
