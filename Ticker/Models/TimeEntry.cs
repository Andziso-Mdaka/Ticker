namespace Ticker.Models;

public class TimeEntry
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ProjectId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? DurationSeconds { get; set; }

    public string? Notes { get; set; }

    public bool IsRunning { get; set; }
}