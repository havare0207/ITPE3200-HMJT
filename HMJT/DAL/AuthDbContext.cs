using HMJT.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HMJT.DAL;

// Database context used for registered users,
// login information and authentication data.
public class AuthDbContext
    : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(
        DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }
}