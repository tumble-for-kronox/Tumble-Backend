using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using WebAPIModels.RequestModels;
using KronoxService.ActionFilters;

namespace KronoxService.Controllers;

[ApiController]
[KronoxUrlFilter]
[Route("api/schedules")]
public class ScheduleController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;

    public ScheduleController(ILogger<ScheduleController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{scheduleId}")]
    public async Task<IActionResult> GetSingleSchedule([FromRoute] string scheduleId, [FromQuery] SchoolEnum schoolId, [FromQuery] string? startDateISO = null)
    {
        return BadRequest();
    }

    [HttpPost("multi")]
    public async Task<IActionResult> GetMultipleSchedules([FromBody] MultiSchoolSchedules[] schoolSchedules, [FromQuery] string? startDateISO = null)
    {
        return BadRequest();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string searchQuery, [FromQuery] SchoolEnum? schoolId = null)
    {
        return BadRequest();
    }
}