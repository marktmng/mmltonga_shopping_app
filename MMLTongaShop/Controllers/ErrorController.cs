using Microsoft.AspNetCore.Mvc;

namespace MMLTongaShop.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 401:
                    return View("TriggerError401");
                case 403:
                    return View("TriggerError403");
                case 404:
                    return View("TriggerError404");
                case 500:
                    return View("TriggerError500");
                // You can add more cases for other status codes if needed
                default:
                    return View("TriggerUnexpectedError");
            }
        }
    }

}
