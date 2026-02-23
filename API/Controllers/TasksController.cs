using Application.DTOs.Attachment;
using Application.DTOs.Comment;
using Application.DTOs.Task;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetTaskByIdAsync(Guid taskId)
    {
        var result = await _taskService.GetTaskByIdAsync(taskId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("details/{taskId:guid}")]
    public async Task<IActionResult> GetTaskDetailsAsync(Guid taskId)
    {
        var result = await _taskService.GetTaskWithDetailsAsyc(taskId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("project/{projectId:guid}")]
    public async Task<IActionResult> GetProjectTasksAsync(Guid projectId)
    {
        var result = await _taskService.GetProjectTasksAsync(projectId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserTasksAsync(Guid userId)
    {
        var result = await _taskService.GetUserTasksAsync(userId);

        if(!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);    
    }
    
    [HttpGet("overdue/{projectId:guid}")]
    public async Task<IActionResult> GetOverdueTasksAsync(Guid projectId)
    {
        var result = await _taskService.GetOverdueTasksAsync(projectId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("status/{projectId:guid}")]
    public async Task<IActionResult> GetTasksByStatusAsync(Guid projectId, [FromQuery] string status)
    {
        var result = await _taskService.GetTasksByStatusAsync(projectId, status);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("priority/{projectId:guid}")]
    public async Task<IActionResult> GetTasksByPriorityAsync(Guid projectId, [FromQuery] string priority)
    {
        var result = await _taskService.GetTasksByPriorityAsync(projectId, priority);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("subtasks/{parentTaskId:guid}")]
    public async Task<IActionResult> GetSubtasksAsync(Guid parentTaskId)
    {
        var result = await _taskService.GetSubtasksAsync(parentTaskId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("assign/{taskId:guid}/{userId:guid}")]
    public async Task<IActionResult> AssignTaskToUserAsync(Guid taskId, Guid userId)
    {
        var result = await _taskService.AssignTaskAsync(taskId, userId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{taskId:guid}/subtasks")]
    public async Task<IActionResult> AddSubtaskAsync(Guid taskId, [FromBody] CreateTaskDto createTaskDto)
    {
        var result = await _taskService.AddSubtaskAsync(taskId, createTaskDto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
    
    [HttpPost("{taskId:guid}/attachments")]
    public async Task<IActionResult> AddAttachmentToTaskAsync(
        Guid taskId,
        [FromBody] CreateAttachmentDto createAttachmentDto)
    {
        var result = await _taskService.AddAttachmentToTaskAsync(taskId, createAttachmentDto);

        if(!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);    
    }
    
    [HttpPost("{taskId:guid}/comments")]
    public async Task<IActionResult> AddCommentToTaskAsync(
        Guid taskId,
        [FromBody] CreateCommentDto createCommentDto)
    {
        var result = await _taskService.AddCommentToTaskAsync(taskId, createCommentDto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTaskAsync([FromBody] CreateTaskDto createTaskDto)
    {
        var result = await _taskService.CreateTaskAsync(createTaskDto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("{taskId:guid}")]
    public async Task<IActionResult> UpdateTaskAsync(Guid taskId, [FromBody] UpdateTaskDto updateTaskDto)
    {
        var result = await _taskService.UpdateTaskAsync(taskId, updateTaskDto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTaskAsync(Guid taskId)
    {
        var result = await _taskService.DeleteTaskAsync(taskId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{taskId:guid}/attachments/{attachmentId:guid}")]
    public async Task<IActionResult> RemoveAttachmentFromTaskAsync(Guid taskId, Guid attachmentId)
    {
        var result = await _taskService.RemoveAttachmentFromTaskAsync(taskId, attachmentId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{taskId:guid}/comments/{commentId:guid}")]
    public async Task<IActionResult> RemoveCommentFromTaskAsync(Guid taskId, Guid commentId)
    {
        var result = await _taskService.RemoveCommentFromTaskAsync(taskId, commentId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}