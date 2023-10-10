using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private IHttpContextAccessor _httpContextAccessor;

        public string sessionCount;

        public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
            Console.WriteLine("Http request happened.");
            
            _httpContextAccessor.HttpContext.Session.SetString("Count", "0");
            
            sessionCount = _httpContextAccessor.HttpContext.Session.GetString("Count");

            Console.WriteLine($"Get Count {sessionCount} from http context session.");
        }

        public void OnPostIncrement()
        {
            // This function will be executed when the button is clicked
            IncrementCounter();
        }

        private void IncrementCounter()
        {
            int n = Int32.Parse(_httpContextAccessor.HttpContext.Session.GetString("Count")) + 1;

            _httpContextAccessor.HttpContext.Session.SetString("Count", n.ToString());

            Console.WriteLine($"Count in http context session has been changed to {n} in the Counter.");

            sessionCount = _httpContextAccessor.HttpContext.Session.GetString("Count");
        }
    }
}