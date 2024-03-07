using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using StripeInegration.Models;
using static System.Net.WebRequestMethods;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace StripeInegration.Controllers
{
    public class CheckOutController : Controller
    {
        public IActionResult Index()
        {
            List<Product> ProductList = new List<Product>()
            {
                new Product()
                {
                    Name = "Burger",
                    Price = 1558,
                    Quanity = 5,
                    ImageUrl = "https://dotnetresturant.blob.core.windows.net/manger/pexels-valeria-boltneva-1639565.jpg"
                },
                new Product()
                {
                    Name = "Pizza",
                    Price = 15789,
                    Quanity = 2,
                    ImageUrl = "https://dotnetresturant.blob.core.windows.net/manger/pexels-polina-tankilevitch-4109998.jpg"
                }
            };
            return View(ProductList);
        }

        public IActionResult OrderConfigration()
        {
            var service =new SessionService();
            Session session = service.Get(TempData["Session"].ToString());
            if (session.PaymentStatus=="paid")
            {
                var transaction = session.PaymentIntentId.ToString();
                return View("Sucess");
            }

            return View("Login");
        }

        public IActionResult Sucess()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        public IActionResult Checkout()
        {
            List<Product> ProductList = new List<Product>()
            {
                new Product()
                {
                    Name = "Burger",
                    Price = 1558,
                    Quanity = 5,
                    ImageUrl = "https://dotnetresturant.blob.core.windows.net/manger/pexels-valeria-boltneva-1639565.jpg"
                },
                new Product()
                {
                    Name = "Pizza",
                    Price = 15789,
                    Quanity = 2,
                    ImageUrl = "https://dotnetresturant.blob.core.windows.net/manger/pexels-polina-tankilevitch-4109998.jpg"
                }
            };
            var domain = "http://localhost:5134/";
            var options = new SessionCreateOptions()
            {
                SuccessUrl = domain+$"Checkout/OrderConfigration",
                CancelUrl = domain+$"Checkout/Login",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "mo.zonkol@gmail.com",
            };
            foreach (var item in ProductList)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * item.Quanity),
                        Currency = "inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name.ToString()
                        }
                    },
                    Quantity = item.Quanity

                };
                options.LineItems.Add(sessionListItem);
                var services =new SessionService();
                Session session = services.Create(options);
                TempData["Session"] = session.Id;
                Response.Headers.Add("Location",session.Url);
                return new StatusCodeResult(303);
            }
            return View();
        }
    }
}
