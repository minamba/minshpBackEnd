using Microsoft.AspNetCore.Mvc;

namespace MinshpWebApp.Api.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
