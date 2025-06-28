using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Shared.Entities;

public class User : IdentityUser<Guid>
{
    public bool IsPrivate { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 