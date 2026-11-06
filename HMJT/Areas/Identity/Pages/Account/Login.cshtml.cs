using System.ComponentModel.DataAnnotations;

using HMJT.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HMJT.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    // ASP.NET Core Identity service used to authenticate users
    // and manage login sessions.
    private readonly SignInManager<ApplicationUser>
        _signInManager;


    // SignInManager is provided through dependency injection.
    public LoginModel(
        SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }


    // Binds the login form fields to this model
    // when the form is submitted.
    [BindProperty]
    public InputModel Input { get; set; } =
        new();


    // Stores any configured external login providers.
    // The current application can still function without them.
    public IList<AuthenticationScheme>
        ExternalLogins
    { get; set; } =
        new List<AuthenticationScheme>();


    // Stores the page the user should return to
    // after a successful login.
    public string? ReturnUrl { get; set; }


    // Allows an error message to survive a redirect
    // and be displayed on the login page.
    [TempData]
    public string? ErrorMessage { get; set; }


    // Contains the values submitted through the login form.
    public class InputModel
    {
        // Email is required and must use a valid email format.
        [Required]
        [EmailAddress]
        public string Email { get; set; } =
            string.Empty;


        // Password is required and rendered as a password field.
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } =
            string.Empty;


        // Determines whether the authentication session
        // should remain active between browser sessions.
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    // Runs when the login page is opened.
    public async Task OnGetAsync(
        string? returnUrl = null)
    {
        // Display a temporary error message if one
        // was supplied by another request.
        if (!string.IsNullOrEmpty(
                ErrorMessage))
        {
            ModelState.AddModelError(
                string.Empty,
                ErrorMessage
            );
        }

        // Return to the home page when no specific
        // return URL has been supplied.
        returnUrl ??= Url.Content("~/");

        // Clears temporary external authentication information
        // left from previous authentication attempts.
        await HttpContext.SignOutAsync(
            IdentityConstants
                .ExternalScheme
        );

        // Retrieves configured external authentication providers.
        ExternalLogins =
            (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
            .ToList();

        ReturnUrl = returnUrl;
    }


    // Runs when the login form is submitted.
    public async Task<IActionResult>
        OnPostAsync(
            string? returnUrl = null)
    {
        // Use the home page as the default destination
        // after a successful login.
        returnUrl ??=
            Url.Content("~/");

        // Retrieve configured external login providers
        // in case the page must be redisplayed.
        ExternalLogins =
            (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
            .ToList();


        // Stop processing if the submitted form
        // does not pass validation.
        if (!ModelState.IsValid)
        {
            return Page();
        }


        // Attempt to authenticate the user.
        //
        // lockoutOnFailure is enabled so repeated failed
        // login attempts contribute to account lockout.
        var result =
            await _signInManager
                .PasswordSignInAsync(
                    Input.Email,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: true
                );


        // Redirect the user after successful authentication.
        if (result.Succeeded)
        {
            return LocalRedirect(
                returnUrl
            );
        }


        // Display a generic message if Identity reports
        // that the account is temporarily locked.
        if (result.IsLockedOut)
        {
            ModelState.AddModelError(
                string.Empty,
                "This account is temporarily locked."
            );

            return Page();
        }


        // Use a generic login error so the response does not reveal
        // whether the email address or password was incorrect.
        ModelState.AddModelError(
            string.Empty,
            "Invalid login attempt."
        );

        return Page();
    }
}