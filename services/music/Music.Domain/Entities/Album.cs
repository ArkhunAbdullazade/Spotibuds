using System.ComponentModel.DataAnnotations;
using Shared;
using Shared.Entities;

namespace Music.Domain.Entities;

public class Album : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ArtistName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? CoverUrl { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public Guid ArtistId { get; set; }
    public virtual Artist Artist { get; set; } = null!;

    // Navigation properties - only within Music service
    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
} 