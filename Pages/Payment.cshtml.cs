using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Stripe.Checkout;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

public class PaymentModel:PageModel {
    private readonly CS3750Assignment1Context _context;

    public PaymentModel(CS3750Assignment1Context context) {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public decimal Amount { get; set; }

    public IActionResult OnGet() {
        if (Amount <= 0) {
            return RedirectToPage("/Tuition"); // Redirect back if no valid amount is provided
        }

        return Page();
    }

    public IActionResult OnPost(int registrationId) {
        StripeConfiguration.ApiKey = "sk_test_51Qqz3pIO8IZgSVGO3DPBClxncEHaYa8eA13FM6Ddwvnmt0itPAekJMVw0zrYOEA7HAbpq6i1ba80I9aG97TOCXIr00nPJqlZSr"; // Replace with your Stripe Secret Key

        var options = new SessionCreateOptions {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Tuition Payment"
                        },
                        UnitAmount = (long)(Amount * 100) // Convert dollars to cents
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = $"{Request.Scheme}://{Request.Host}/Success",
            CancelUrl = $"{Request.Scheme}://{Request.Host}/Cancel"
        };

        var service = new SessionService();
        Session session = service.Create(options);

        // Update the IsPaid property in the Registration model
        var registration = _context.Registration.FirstOrDefault(r => r.Id == registrationId);
        if (registration != null) {
            registration.IsPaid = true;
            _context.Attach(registration).State = EntityState.Modified;
            _context.SaveChanges();
        }

        return Redirect(session.Url);
    }
}