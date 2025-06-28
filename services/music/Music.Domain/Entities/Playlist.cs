using System.ComponentModel.DataAnnotations;
using Shared;
using Shared.Entities;

namespace Music.Domain.Entities;

public class Playlist : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid OwnerId { get; set; }

    // Navigation properties - only within Music service
    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
} 