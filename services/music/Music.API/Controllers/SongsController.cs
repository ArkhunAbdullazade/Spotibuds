using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music.Domain.Data;
using Music.Domain.Entities;

namespace Music.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly MusicDbContext context;

    public SongsController(MusicDbContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
    {
        return await this.context.Songs.Include(s => s.Album).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Song>> GetSong(Guid id)
    {
        var song = await this.context.Songs.Include(s => s.Album).FirstOrDefaultAsync(s => s.Id == id);
        if (song == null)
        {
            return NotFound();
        }
        return song;
    }

    [HttpPost]
    public async Task<ActionResult<Song>> CreateSong(Song song)
    {
        this.context.Songs.Add(song);
        await this.context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSong), new { id = song.Id }, song);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSong(Guid id, Song song)
    {
        if (id != song.Id)
        {
            return BadRequest();
        }

        this.context.Entry(song).State = EntityState.Modified;
        await this.context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSong(Guid id)
    {
        var song = await this.context.Songs.FindAsync(id);
        if (song == null)
        {
            return NotFound();
        }

        this.context.Songs.Remove(song);
        await this.context.SaveChangesAsync();
        return NoContent();
    }
}