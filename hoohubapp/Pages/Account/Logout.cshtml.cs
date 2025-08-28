// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using hoohub.Configuration;
using hoohub.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hoohub.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<HooHubUser> _signInManager;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Initialises a new instance of the <see cref="LoginModel"/> class.
        /// </summary>
        /// <param name="signInManager">Injected <see cref="SignInManager{TUser}"/> service.</param>
        /// <param name="appSettings">Injected <see cref="AppSettings"/>.</param>
        public LogoutModel(SignInManager<HooHubUser> signInManager, AppSettings appSettings)
        {
            _signInManager = signInManager;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Initiates the logout process, redirecting the user.
        /// </summary>
        /// <param name="returnUrl">Optional URL for redirection.</param>
        /// <returns>The page defined by the return URL if given, otherwise this page.</returns>
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}