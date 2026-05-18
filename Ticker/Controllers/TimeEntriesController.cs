using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Ticker.Data;
using Ticker.Models;

namespace Ticker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeEntriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TimeEntriesController(AppDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue("sub")!);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var userId = GetUserId();

        var active = await _context.TimeEntries
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsRunning);

        return Ok(active);
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] Guid projectId)
    {
        var userId = GetUserId();

        var running = await _context.TimeEntries
            .AnyAsync(x => x.UserId == userId && x.IsRunning);

        if (running)
            return BadRequest("Timer already running");

        var entry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectId = projectId,
            StartTime = DateTime.UtcNow,
            IsRunning = true
        };

        _context.TimeEntries.Add(entry);
        await _context.SaveChangesAsync();

        return Ok(entry);
    }

    [HttpPost("stop")]
    public async Task<IActionResult> Stop()
    {
        var userId = GetUserId();

        var entry = await _context.TimeEntries
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsRunning);

        if (entry == null)
            return BadRequest("No active timer");

        entry.EndTime = DateTime.UtcNow;
        entry.IsRunning = false;
        entry.DurationSeconds =
            (int)(entry.EndTime.Value - entry.StartTime).TotalSeconds;

        await _context.SaveChangesAsync();

        return Ok(entry);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();

        var entries = await _context.TimeEntries
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.StartTime)
            .ToListAsync();

        return Ok(entries);
    }
}