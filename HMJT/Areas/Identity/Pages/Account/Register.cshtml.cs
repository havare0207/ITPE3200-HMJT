using System.ComponentModel.DataAnnotations;

using HMJT.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HMJT.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    // ASP.NET Core Identity service used to create
    // and manage ApplicationUser accounts.
    private readonly UserManager<ApplicationUser>
        _userManager;

    // Identity service used to sign in the user
    // after successful registration.
    private readonly SignInManager<ApplicationUser>
        _signInManager;


    // Identity services are provided through dependency injection.
    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }


    // Binds registration form fields to this model
    // when the form is submitted.
    [BindProperty]
    public InputModel Input { get; set; } =
        new();


    // Stores the destination the user should return to
    // after successful registration.
    public string? ReturnUrl { get; set; }


    // Contains all values entered into the registration form.
    public class InputModel
    {
        // Email is required and must use a valid email format.
        [Required]
        [EmailAddress]
        public string Email { get; set; } =
            string.Empty;


        // Password must satisfy this model's length requirement.
        // Additional ASP.NET Core Identity password requirements
        // may also be configured elsewhere in the application.
        [Required]
        [StringLength(
            100,
            ErrorMessage =
                "The {0} must be at least {2} and at most {1} characters long.",
            MinimumLength = 10)]
        [DataType(DataType.Password)]
        public string Password { get; set; } =
            string.Empty;


        // Confirmation is compared with the Password property
        // before account creation can continue.
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(
            "Password",
            ErrorMessage =
                "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } =
            string.Empty;
    }


    // Runs when the registration page is opened.
    public void OnGet(
        string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }


    // Runs when the registration form is submitted.
    public async Task<IActionResult> OnPostAsync(
        string? returnUrl = null)
    {
        // Return to the home page when no other
        // destination has been supplied.
        returnUrl ??=
            Url.Content("~/");


        // Stop account creation when validation fails.
        if (!ModelState.IsValid)
        {
            return Page();
        }


        // Creates a new application user using the submitted email
        // as both the username and email address.
        var user =
            new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email
            };


        // ASP.NET Core Identity creates the account and securely
        // handles password hashing and storage.
        IdentityResult result =
            await _userManager
                .CreateAsync(
                    user,
                    Input.Password
                );


        // Sign the new user in immediately after
        // successful account creation.
        if (result.Succeeded)
        {
            await _signInManager
                .SignInAsync(
                    user,
                    isPersistent: false
                );

            return LocalRedirect(
                returnUrl
            );
        }


        // Add Identity errors to ModelState so they
        // can be displayed by the validation summary.
        foreach (IdentityError error
                 in result.Errors)
        {
            ModelState.AddModelError(
                string.Empty,
                error.Description
            );
        }

        return Page();
    }
}