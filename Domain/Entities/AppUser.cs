using Microsoft.AspNetCore.Identity;

namespace AuthImplementation.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}