using System.ComponentModel.DataAnnotations;
using Shared;

namespace User.Domain.Entities;

public class Follow : BaseEntity
{
    [Required]
    public Guid FollowerId { get; set; }

    [Required]
    public Guid FollowedId { get; set; }
} 