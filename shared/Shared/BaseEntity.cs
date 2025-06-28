using System.ComponentModel.DataAnnotations;

namespace Shared;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ReactionType
{
    Like,
    Love,
    Fire,
    Heart,
    ThumbsUp
}
