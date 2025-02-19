using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<CS3750Assignment1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CS3750Assignment1Context") ?? throw new InvalidOperationException("Connection string 'CS3750Assignment1Context' not found.")));

Stripe.StripeConfiguration.ApiKey = "sk_test_51Qqz3pIO8IZgSVGO3DPBClxncEHaYa8eA13FM6Ddwvnmt0itPAekJMVw0zrYOEA7HAbpq6i1ba80I9aG97TOCXIr00nPJqlZSr";

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions {
    MinimumSameSitePolicy = SameSiteMode.Strict
});

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
