using System.ComponentModel.DataAnnotations;
using Shared;
using Shared.Entities;

namespace Identity.Domain.Entities;

public class RefreshToken : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;

    // Navigation properties
    public virtual Shared.Entities.User User { get; set; } = null!;
} 