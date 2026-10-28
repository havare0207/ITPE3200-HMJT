using System.ComponentModel.DataAnnotations;

using HMJT.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HMJT.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser>
        _signInManager;

    public LoginModel(
        SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }


    [BindProperty]
    public InputModel Input { get; set; } =
        new();


    public IList<AuthenticationScheme>
        ExternalLogins { get; set; } =
        new List<AuthenticationScheme>();


    public string? ReturnUrl { get; set; }


    [TempData]
    public string? ErrorMessage { get; set; }


    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } =
            string.Empty;


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } =
            string.Empty;


        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public async Task OnGetAsync(
        string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(
                ErrorMessage))
        {
            ModelState.AddModelError(
                string.Empty,
                ErrorMessage
            );
        }

        returnUrl ??= Url.Content("~/");

        await HttpContext.SignOutAsync(
            IdentityConstants
                .ExternalScheme
        );

        ExternalLogins =
            (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
            .ToList();

        ReturnUrl = returnUrl;
    }


    public async Task<IActionResult>
        OnPostAsync(
            string? returnUrl = null)
    {
        returnUrl ??=
            Url.Content("~/");

        ExternalLogins =
            (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
            .ToList();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result =
            await _signInManager
                .PasswordSignInAsync(
                    Input.Email,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: true
                );

        if (result.Succeeded)
        {
            return LocalRedirect(
                returnUrl
            );
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(
                string.Empty,
                "This account is temporarily locked."
            );

            return Page();
        }

        ModelState.AddModelError(
            string.Empty,
            "Invalid login attempt."
        );

        return Page();
    }
}