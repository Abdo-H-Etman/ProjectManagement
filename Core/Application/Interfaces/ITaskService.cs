using System.Net.Mail;
using Application.Common.Models;
using Application.DTOs.Attachment;
using Application.DTOs.Comment;
using Application.DTOs.Task;

namespace Application.Interfaces;

public interface ITaskService
{
    Task<Result<TaskDto>> GetTaskByIdAsync(
        Guid taskId,
        CancellationToken cancellationToken = default);
    Task<Result<TaskDetailsDto>> GetTaskWithDetailsAsyc(
        Guid taskId,
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TaskDto>>> GetProjectTasksAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TaskDto>>> GetUserTasksAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TaskDto>>> GetOverdueTasksAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TaskDto>>> GetTasksByStatusAsync(
        Guid projectId,
        string? status,   
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TaskDto>>> GetTasksByPriorityAsync(
        Guid projectId,
        string? priority,
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TaskDto>>> GetSubtasksAsync(
        Guid parentTaskId,
        CancellationToken cancellationToken = default);
    Task<Result<TaskDto>> CreateTaskAsync(
        CreateTaskDto createTaskDto,
        CancellationToken cancellationToken = default);
    Task<Result> AssignTaskAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken = default);    
    Task<Result<TaskDto>> UpdateTaskAsync(
        Guid taskId,
        UpdateTaskDto updateTaskDto,
        CancellationToken cancellationToken = default);
    Task<Result> DeleteTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken = default);
    Task<Result<TaskDetailsDto>> AddSubtaskAsync(
        Guid parentTaskId,
        CreateTaskDto createTaskDto,
        CancellationToken cancellationToken = default);
    Task<Result<TaskDetailsDto>> AddAttachmentToTaskAsync(
        Guid taskId,
        CreateAttachmentDto createAttachmentDto,
        CancellationToken cancellationToken = default);
    Task<Result> RemoveAttachmentFromTaskAsync(
        Guid taskId,
        Guid attachmentId,
        CancellationToken cancellationToken = default);
    Task<Result<TaskDetailsDto>> AddCommentToTaskAsync(
        Guid taskId,
        CreateCommentDto createCommentDto,
        CancellationToken cancellationToken = default);
    Task<Result> RemoveCommentFromTaskAsync(
        Guid taskId,
        Guid commentId,
        CancellationToken cancellationToken = default);
}
