using Application.Common.Models;
using Application.Common.Models.Interfaces;
using Application.DTOs.Task;
using Application.Interfaces;
using Application.Interfaces.Logging;
using Application.Interfaces.Mailing;
using AutoMapper;
using Domain.Entities.Enums;
using Domain.Interfaces;
using TaskStatus = Domain.Entities.Enums.TaskStatus;
using Task = Domain.Entities.Models.Task;
using Application.DTOs.Attachment;
using Domain.Entities.Models;
using Application.DTOs.Comment;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public TaskService(
        IRepositoryManager repositoryManager,
        ICurrentUserService currentUserService,
        ILoggerManager logger,
        IEmailService emailService,
        IMapper mapper)
    {
        _repositoryManager = repositoryManager;
        _currentUserService = currentUserService;
        _logger = logger;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<Result<TaskDto>> GetTaskByIdAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var task = await _repositoryManager.Task.GetByIdAsync(
            taskId,
            cancellationToken);

            if (task is null)
            {
                _logger.LogWarn("Task with id: {taskId} not found.", taskId);
                return Result<TaskDto>.Failure($"Task with ID: {taskId} not found.");
            }

            var taskDto = _mapper.Map<TaskDto>(task);

            _logger.LogInfo("Retrieved task with ID: {taskId} successfully.", taskId);
            return Result<TaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving task with ID: {taskId}. Error: {message}", taskId, ex.Message);
            return Result<TaskDto>.Failure("An unexpected error occurred. Please try again later.");
        }
    }
    
    public async Task<Result<TaskDetailsDto>> GetTaskWithDetailsAsyc(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var task = await _repositoryManager.Task.GetTaskWithDetailsAsync(
                taskId,
                cancellationToken);

            if (task is null)
            {
                _logger.LogWarn("Task with ID: {taskId} not found.", taskId);
                return Result<TaskDetailsDto>.Failure($"Task with id: {taskId} not found.");
            }

            var taskDetailsDto = _mapper.Map<TaskDetailsDto>(task);

            _logger.LogInfo("Retrieved details for task with ID: {taskId} successfully.", taskId);
            return Result<TaskDetailsDto>.Success(taskDetailsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving task details with id: {taskId}. Error: {message}",
                                taskId, ex.Message);
            return Result<TaskDetailsDto>.Failure($"An unexpected error occurred. Please try again later. {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TaskDto>>> GetProjectTasksAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetByIdAsync(
                projectId,
                cancellationToken);

            if (project is null)
            {
                _logger.LogWarn("Project with id: {projectId} not found.", projectId);
                return Result<IEnumerable<TaskDto>>.Failure($"Project with id: {projectId} not found.");
            }    

            var tasks = await _repositoryManager.Task.GetProjectTasksAsync(
                projectId,
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo("Retrieved tasks for project with ID: {projectId} successfully.", projectId);
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving tasks for project id: {projectId}. Error: {message}",
                                projectId, ex.Message);
            return Result<IEnumerable<TaskDto>>.Failure("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<Result<IEnumerable<TaskDto>>> GetUserTasksAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repositoryManager.User.GetByIdAsync(
                userId,
                cancellationToken);

            if (user is null)
            {
                _logger.LogWarn("User with ID: {userId} not found.", userId);
                return Result<IEnumerable<TaskDto>>.Failure($"User with id: {userId} not found.");
            }    

            var tasks = await _repositoryManager.Task.GetUserTasksAsync(
                userId,
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo("Retrieved tasks for user with ID: {userId} successfully.", userId);
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving tasks for user id: {userId}. Error: {message}",
                                userId, ex.Message);
            return Result<IEnumerable<TaskDto>>.Failure("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<Result<IEnumerable<TaskDto>>> GetOverdueTasksAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetByIdAsync(
                projectId,
                cancellationToken);

            if (project is null)
            {
                _logger.LogWarn("Project with ID: {projectId} not found.", projectId);
                return Result<IEnumerable<TaskDto>>.Failure($"Project with id: {projectId} not found.");
            }

            var tasks = await _repositoryManager.Task.GetOverdueTasksAsync(
                projectId,
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo("Retrieved overdue tasks for project with ID: {projectId} successfully.", projectId);
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving overdue tasks for project id: {projectId}. Error: {message}",
                                projectId, ex.Message);
            return Result<IEnumerable<TaskDto>>.Failure("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<Result<IEnumerable<TaskDto>>> GetTasksByStatusAsync(
        Guid projectId,
        string? status,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetByIdAsync(
                projectId,
                cancellationToken);

            if( project is null)
            {
                _logger.LogWarn("Project with id: {projectId} not found.", projectId);
                return Result<IEnumerable<TaskDto>>.Failure($"Project with id: {projectId} not found.");
            }

            var tasks = await _repositoryManager.Task.GetTasksByStatusAsync(
                projectId,
                Enum.Parse<TaskStatus>(status ?? "Pending"),
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo("Tasks in project with ID {projectId} with status {status} are retrieved successfuly.",
                                project.Id, status!);
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);     
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving tasks by status: {status} for project ID: {projectId}. Error: {message}",
                                status ?? "Pending", projectId, ex.Message);
            return Result<IEnumerable<TaskDto>>.Failure("An unexpected error occurred. Please try again later");
        }
    }

    public async Task<Result<IEnumerable<TaskDto>>> GetTasksByPriorityAsync(
        Guid projectId,
        string? priority,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetByIdAsync(
                projectId,
                cancellationToken);

            if( project is null)
            {
                _logger.LogWarn("Project with ID: {projectId} not found.", projectId);
                return Result<IEnumerable<TaskDto>>.Failure($"Project with id: {projectId} not found.");
            }

            var tasks = await _repositoryManager.Task.GetTasksByPriorityAsync(
                projectId,
                Enum.Parse<TaskPriority>(priority ?? "Urgent"),
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo("Tasks in project with ID: {projectId} with priority {priority} are retrieved successfuly.",
                                projectId, priority ?? "Urgent");
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);     
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving tasks by priority: {priority} for project id: {projectId}. Error: {message}",
                                priority ?? "Urgent", projectId, ex.Message);
            return Result<IEnumerable<TaskDto>>.Failure("An unexpected error occurred. Please try again later");
        }
    }

    public async Task<Result<IEnumerable<TaskDto>>> GetSubtasksAsync(
        Guid parentTaskId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var parentTask = await _repositoryManager.Task.GetByIdAsync(
                parentTaskId,
                cancellationToken);

            if (parentTask is null)
            {
                _logger.LogWarn("Parent task with id: {parentTaskId} not found.", parentTaskId);
                return Result<IEnumerable<TaskDto>>.Failure($"Parent task with id: {parentTaskId} not found.");
            }    

            var subtasks = await _repositoryManager.Task.GetSubtasksAsync(
                parentTaskId,
                cancellationToken);

            var subtaskDtos = _mapper.Map<IEnumerable<TaskDto>>(subtasks);

            _logger.LogInfo("Retrieved subtasks for parent task id: {parentTaskId} successfully.", parentTaskId);
            return Result<IEnumerable<TaskDto>>.Success(subtaskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving subtasks for parent task with ID: {parentTaskId}. Error: {message}",
                                parentTaskId, ex.Message);
            return Result<IEnumerable<TaskDto>>.Failure("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<Result<TaskDto>> CreateTaskAsync(
        CreateTaskDto createTaskDto,
        CancellationToken cancellationToken = default)
    {
        var task = _mapper.Map<Task>(createTaskDto);
        task.CreatedById = _currentUserService.UserId;

        await _repositoryManager.Task.AddAsync(
            task,
            cancellationToken);
        await _repositoryManager.SaveAsync(cancellationToken);

        var taskDto = _mapper.Map<TaskDto>(task);

        _logger.LogInfo("Task with ID: {taskId} created successfully.", task.Id);
        return Result<TaskDto>.Success(taskDto);
    }

    public async Task<Result<TaskDto>> UpdateTaskAsync(
        Guid taskId,
        UpdateTaskDto updateTaskDto,
        CancellationToken cancellationToken = default)
    {
        var task = await _repositoryManager.Task.GetByIdAsync(
            taskId,
            cancellationToken);

        if (task is null)
        {
            _logger.LogWarn("Task with id: {taskId} not found.", taskId);
            return Result<TaskDto>.Failure($"Task with id: {taskId} not found.");
        }

        _mapper.Map(updateTaskDto, task);
        task.UpdatedAt = DateTime.UtcNow;
        if(task.Status == TaskStatus.Completed && task.CompletedAt == null)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        
        await _repositoryManager.SaveAsync(cancellationToken);

        var taskDto =  _mapper.Map<TaskDto>(task);

        _logger.LogInfo("Task with ID: {taskId} updated successfully.", taskId);
        return Result<TaskDto>.Success(taskDto);
    }

    public async Task<Result> DeleteTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _repositoryManager.Task.GetByIdAsync(
            taskId,
            cancellationToken);

        if (task is null)
        {
            _logger.LogWarn("Task with ID: {taskId} not found.", taskId);
            return Result.Failure($"Task with id: {taskId} not found.");
        }

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;

        await _repositoryManager.SaveAsync(cancellationToken);

        _logger.LogInfo("Task with ID: {taskId} deleted successfully.", taskId);
        return Result.Success();
    }

    public async Task<Result> AssignTaskAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var task = await _repositoryManager.Task.GetByIdAsync(
            taskId,
            cancellationToken);

        if (task is null)
        {
            _logger.LogWarn("Task with ID: {taskId} not found.", taskId);
            return Result.Failure($"Task with ID: {taskId} not found.");
        }

        task.AssignedToId = userId;
        task.AssignedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        await _repositoryManager.SaveAsync(cancellationToken);

        var user = await _repositoryManager.User.GetByIdAsync(
            userId,
            cancellationToken);

        if (user is not null)
        {
            var assignerName = _currentUserService.UserName ?? "Someone";
            var taskLink = $"https://app.example.com/tasks/{taskId}";

            await _emailService.SendTaskAssignmentEmailAsync(
                toEmail: user.Email!,
                toName: user.GetFullName(),
                taskName: task.Title,
                projectName: (await _repositoryManager.Project.GetByIdAsync(
                    task.ProjectId,
                    cancellationToken))?.Name ?? "Project",
                assignedByName: assignerName,
                taskLink: taskLink,
                dueDate: task.DueDate,
                cancellationToken);
        }

        _logger.LogInfo("Task with ID: {taskId} assigned to user with ID: {userId} successfully.", taskId, userId);
        return Result.Success("Task assigned successfully.");
    }

    public async Task<Result<TaskDetailsDto>> AddSubtaskAsync(
        Guid parentTaskId,
        CreateTaskDto createTaskDto,
        CancellationToken cancellationToken = default)
    {
        var parentTask = await _repositoryManager.Task.GetByIdAsync(
            parentTaskId,
            cancellationToken);

        if (parentTask is null)
        {
            _logger.LogWarn("Parent task with ID: {parentTaskId} not found.", parentTaskId);
            return Result<TaskDetailsDto>.Failure($"Parent task with ID: {parentTaskId} not found.");
        }

        var subtask = _mapper.Map<Task>(createTaskDto);
        subtask.ParentTaskId = parentTaskId;
        subtask.CreatedById = _currentUserService.UserId;

        await _repositoryManager.Task.AddAsync(
            subtask,
            cancellationToken);
        await _repositoryManager.SaveAsync(cancellationToken);

        var updatedParentTask = await _repositoryManager.Task.GetTaskWithDetailsAsync(
            parentTaskId,
            cancellationToken);

        var taskDetailsDto = _mapper.Map<TaskDetailsDto>(updatedParentTask);

        _logger.LogInfo("subtask with ID: {subtaskId} added  to parent task ID: {parentTaskId} successfully.", subtask.Id,
                            parentTaskId);
        return Result<TaskDetailsDto>.Success(taskDetailsDto);
    }

    public async Task<Result<TaskDetailsDto>> AddAttachmentToTaskAsync(
        Guid taskId,
        CreateAttachmentDto createAttachmentDto,
        CancellationToken cancellationToken = default)
    {
        var task = await _repositoryManager.Task.GetByIdAsync(
            taskId,
            cancellationToken);

        if (task is null)
        {
            _logger.LogWarn("Task with ID: {taskId} not found.", taskId);
            return Result<TaskDetailsDto>.Failure($"Task with ID: {taskId} not found.");
        }

        var attachment = _mapper.Map<Attachment>(createAttachmentDto);
        attachment.TaskId = taskId;
        attachment.UploadedById = _currentUserService.UserId;

        await _repositoryManager.Attachment.AddAsync(
            attachment,
            cancellationToken);
        await _repositoryManager.SaveAsync(cancellationToken);

        var updatedTask = await _repositoryManager.Task.GetTaskWithDetailsAsync(
            taskId,
            cancellationToken);

        var taskDetailsDto = _mapper.Map<TaskDetailsDto>(updatedTask);

        _logger.LogInfo("Attachment with ID: {attachmentId} added to task with ID: {taskId} successfully.", attachment.Id,
                            taskId);
        return Result<TaskDetailsDto>.Success(taskDetailsDto);
    }

    public async Task<Result> RemoveAttachmentFromTaskAsync(
        Guid taskId,
        Guid attachmentId,
        CancellationToken cancellationToken = default)
    {
        var task = await _repositoryManager.Task.GetByIdAsync(
            taskId,
            cancellationToken);

        if (task is null)
        {
            _logger.LogWarn("Task with ID: {taskId} not found.", taskId);
            return Result.Failure($"Task with ID: {taskId} not found.");
        }

        var attachment = await _repositoryManager.Attachment.GetByIdAsync(
            attachmentId,
            cancellationToken);

        if (attachment is null || attachment.TaskId != taskId)
        {
            _logger.LogWarn("Attachment with ID: {attachmentId} not found for task ID: {taskId}.", attachmentId, taskId);
            return Result.Failure($"Attachment with ID: {attachmentId} not found for task ID: {taskId}.");
        }

         _repositoryManager.Attachment.Remove(
            attachment);
        await _repositoryManager.SaveAsync(cancellationToken);

        _logger.LogInfo("Removed attachment with ID: {attachmentId} from task ID: {taskId} successfully.", attachmentId, taskId);
        return Result.Success();
    }

    public async Task<Result<TaskDetailsDto>> AddCommentToTaskAsync(
        Guid taskId,
        CreateCommentDto createCommentDto,
        CancellationToken cancellationToken = default)
    {
        var task = await _repositoryManager.Task.GetByIdAsync(
            taskId,
            cancellationToken);
        
        if (task is null)
        {
            _logger.LogWarn("Task with ID: {taskId} not found.", taskId);
            return Result<TaskDetailsDto>.Failure($"Task with ID: {taskId} not found.");
        }

        var comment = _mapper.Map<Comment>(createCommentDto);
        comment.TaskId = taskId;
        comment.AuthorId = _currentUserService.UserId;

        await _repositoryManager.Comment.AddAsync(
            comment,
            cancellationToken);
        await _repositoryManager.SaveAsync(cancellationToken);

        var updatedTask = await _repositoryManager.Task.GetTaskWithDetailsAsync(
            taskId,
            cancellationToken);

        var taskDetailsDto = _mapper.Map<TaskDetailsDto>(updatedTask);

        _logger.LogInfo("Comment with ID: {commentId} added to task with ID: {taskId} successfully.", comment.Id, taskId);
        return Result<TaskDetailsDto>.Success(taskDetailsDto);
    }

    public async Task<Result> RemoveCommentFromTaskAsync(
        Guid taskId,
        Guid commentId,
        CancellationToken cancellationToken = default)
    {
        var task = await _repositoryManager.Task.GetByIdAsync(
            taskId,
            cancellationToken);

        if (task is null)
        {
            _logger.LogWarn("Task with ID: {taskId} not found.", taskId);
            return Result.Failure($"Task with ID: {taskId} not found.");
        }

        var comment = await _repositoryManager.Comment.GetByIdAsync(
            commentId,
            cancellationToken);

        if (comment is null || comment.TaskId != taskId)
        {
            _logger.LogWarn("Comment with ID: {commentId} not found for task with ID: {taskId}.", commentId, taskId);
            return Result.Failure($"Comment with ID: {commentId} not found for task with ID: {taskId}.");
        }

         _repositoryManager.Comment.Remove(
            comment);
        await _repositoryManager.SaveAsync(cancellationToken);

        _logger.LogInfo("Comment with ID: {commentId} removed from task with ID: {taskId} successfully.", commentId, taskId);
        return Result.Success();
    }
}