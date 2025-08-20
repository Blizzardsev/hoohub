using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hoohub.Pages
{
    [AllowAnonymous]
    public class CookiesModel : PageModel
    {
        /// <summary>
        /// Returns the cookies page.
        /// </summary>
        public void OnGet()
        {
        }
    }
}
