using CS3750Assignment1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CS3750Assignment1.Pages
{
    public class MarkSingleSeenModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public MarkSingleSeenModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        public class NotificationInput
        {
            public int Id { get; set; }
        }

        public async Task<IActionResult> OnPostAsync([FromBody] NotificationInput input)
        {
            if (input?.Id > 0)
            {
                var note = await _context.Notification.FindAsync(input.Id);
                if (note != null && !note.IsSeen)
                {
                    note.IsSeen = true;
                    await _context.SaveChangesAsync();
                    return new JsonResult(new { success = true });
                }
            }

            return new JsonResult(new { success = false });
        }
    }
}
