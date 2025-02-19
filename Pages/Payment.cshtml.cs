using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Stripe.Checkout;

public class PaymentModel:PageModel {
    [BindProperty(SupportsGet = true)]
    public decimal Amount { get; set; }

    public IActionResult OnGet() {
        if (Amount <= 0) {
            return RedirectToPage("/Tuition"); // Redirect back if no valid amount is provided
        }

        return Page();
    }

    public IActionResult OnPost() {
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

        return Redirect(session.Url);
    }
}