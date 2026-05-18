using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticker.Data;
using Ticker.Models;

namespace Ticker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Projects.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Project project)
    {
        project.Id = Guid.NewGuid();
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return Ok(project);
    }
}