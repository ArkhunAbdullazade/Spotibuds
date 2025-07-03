using System;
using System.Collections.Generic;
using Shared;

namespace Music.Domain.Entities
{
    public class Artist : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }

        // Navigation properties
        public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
        public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
    }
} 