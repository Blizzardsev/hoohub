// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using hoohub.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hoohub.Areas.Identity.Pages.Account
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        private readonly AppSettings _appSettings;

        public ForgotPasswordConfirmation(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public async Task<IActionResult> OnGet()
        {
            if (_appSettings.BlockedUserAgents.Contains(Request.Headers["User-Agent"].ToString().ToLower())
                    || _appSettings.BlockedIpAddressRange.Contains(Request.HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                return NotFound("Sorry, we could not process your request: please try again later.");
            }

            return Page();
        }
    }
}
