using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Tasks;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/tasks")]
[Produces("application/json")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>Creates a task.</summary>
    /// <remarks>
    /// Request example:
    /// { "title": "Prepare sprint planning", "description": "Review backlog and define priorities.", "status": "Pending", "dueDate": "2026-07-01T14:00:00Z", "userId": "11111111-1111-1111-1111-111111111111" }
    ///
    /// Response example:
    /// { "id": "22222222-2222-2222-2222-222222222222", "title": "Prepare sprint planning", "description": "Review backlog and define priorities.", "status": "Pending", "dueDate": "2026-07-01T14:00:00Z", "userId": "11111111-1111-1111-1111-111111111111", "user": { "id": "11111111-1111-1111-1111-111111111111", "name": "Ada Lovelace", "email": "ada@example.com" } }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> Create(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var response = await _taskService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>Gets a task by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _taskService.GetByIdAsync(id, cancellationToken));
    }

    /// <summary>Gets all tasks.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TaskResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _taskService.GetAllAsync(cancellationToken));
    }

    /// <summary>Updates a task.</summary>
    /// <remarks>
    /// Request example:
    /// { "title": "Prepare sprint planning", "description": "Include roadmap risks.", "status": "InProgress", "dueDate": "2026-07-02T14:00:00Z", "userId": "11111111-1111-1111-1111-111111111111" }
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> Update(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _taskService.UpdateAsync(id, request, cancellationToken));
    }

    /// <summary>Deletes a task.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _taskService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Gets tasks assigned to a user.</summary>
    [HttpGet("by-user/{userId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<TaskResponse>>> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        return Ok(await _taskService.GetByUserAsync(userId, cancellationToken));
    }
}
