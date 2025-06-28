using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.Domain.Data;
using User.Domain.Entities;

namespace User.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FollowsController : ControllerBase
{
    private readonly UserDbContext _context;

    public FollowsController(UserDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Follow a user
    /// </summary>
    [HttpPost("{targetUserId}/follow")]
    public async Task<IActionResult> FollowUser(Guid targetUserId, [FromBody] Guid currentUserId)
    {
        // Check if already following
        var existingFollow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowedId == targetUserId);

        if (existingFollow != null)
        {
            return BadRequest("Already following this user");
        }

        // Prevent self-following
        if (currentUserId == targetUserId)
        {
            return BadRequest("Cannot follow yourself");
        }

        var follow = new Follow
        {
            FollowerId = currentUserId,
            FollowedId = targetUserId
        };

        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Successfully followed user" });
    }

    /// <summary>
    /// Unfollow a user
    /// </summary>
    [HttpDelete("{targetUserId}/follow")]
    public async Task<IActionResult> UnfollowUser(Guid targetUserId, [FromBody] Guid currentUserId)
    {
        var follow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowedId == targetUserId);

        if (follow == null)
        {
            return NotFound("Not following this user");
        }

        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Successfully unfollowed user" });
    }

    /// <summary>
    /// Get followers of a user
    /// </summary>
    [HttpGet("{userId}/followers")]
    public async Task<ActionResult<IEnumerable<Guid>>> GetFollowers(Guid userId)
    {
        var followerIds = await _context.Follows
            .Where(f => f.FollowedId == userId)
            .Select(f => f.FollowerId)
            .ToListAsync();

        return Ok(followerIds);
    }

    /// <summary>
    /// Get users that a user is following
    /// </summary>
    [HttpGet("{userId}/following")]
    public async Task<ActionResult<IEnumerable<Guid>>> GetFollowing(Guid userId)
    {
        var followingIds = await _context.Follows
            .Where(f => f.FollowerId == userId)
            .Select(f => f.FollowedId)
            .ToListAsync();

        return Ok(followingIds);
    }

    /// <summary>
    /// Check if user A follows user B
    /// </summary>
    [HttpGet("{followerId}/follows/{followedId}")]
    public async Task<ActionResult<bool>> CheckIfFollowing(Guid followerId, Guid followedId)
    {
        var isFollowing = await _context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);

        return Ok(isFollowing);
    }

    /// <summary>
    /// Get follow statistics for a user
    /// </summary>
    [HttpGet("{userId}/stats")]
    public async Task<ActionResult> GetFollowStats(Guid userId)
    {
        var followerCount = await _context.Follows.CountAsync(f => f.FollowedId == userId);
        var followingCount = await _context.Follows.CountAsync(f => f.FollowerId == userId);

        return Ok(new 
        { 
            userId,
            followerCount,
            followingCount
        });
    }
} 