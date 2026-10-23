using Microsoft.AspNetCore.Identity;

namespace HMJT.Models;

// Represents one registered Code Pursuit user.
// Passwords are handled securely by ASP.NET Core Identity
// and are never stored as plain text.
public class ApplicationUser : IdentityUser
{
}