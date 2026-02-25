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
                _logger.LogWarn($"Task with id: {taskId} not found.");
                return Result<TaskDto>.Failure($"Task with id: {taskId} not found.");
            }

            var taskDto = _mapper.Map<TaskDto>(task);

            _logger.LogInfo($"Retrieved task with id: {taskId} successfully.");
            return Result<TaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving task with id: {taskId}. Error: {ex.Message}");
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
                _logger.LogWarn($"Task with id: {taskId} not found.");
                return Result<TaskDetailsDto>.Failure($"Task with id: {taskId} not found.");
            }

            var taskDetailsDto = _mapper.Map<TaskDetailsDto>(task);

            _logger.LogInfo($"Retrieved task details with id: {taskId} successfully.");
            return Result<TaskDetailsDto>.Success(taskDetailsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving task details with id: {taskId}. Error: {ex.Message}");
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
                _logger.LogWarn($"Project with id: {projectId} not found.");
                return Result<IEnumerable<TaskDto>>.Failure($"Project with id: {projectId} not found.");
            }    

            var tasks = await _repositoryManager.Task.GetProjectTasksAsync(
                projectId,
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo($"Retrieved tasks for project id: {projectId} successfully.");
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving tasks for project id: {projectId}. Error: {ex.Message}");
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
                _logger.LogWarn($"User with id: {userId} not found.");
                return Result<IEnumerable<TaskDto>>.Failure($"User with id: {userId} not found.");
            }    

            var tasks = await _repositoryManager.Task.GetUserTasksAsync(
                userId,
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo($"Retrieved tasks for user id: {userId} successfully.");
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving tasks for user id: {userId}. Error: {ex.Message}");
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
                _logger.LogWarn($"Project with id: {projectId} not found.");
                return Result<IEnumerable<TaskDto>>.Failure($"Project with id: {projectId} not found.");
            }    

            var tasks = await _repositoryManager.Task.GetOverdueTasksAsync(
                projectId,
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo($"Retrieved overdue tasks for project id: {projectId} successfully.");
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving overdue tasks for project id: {projectId}. Error: {ex.Message}");
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
                _logger.LogWarn($"Project with id: {projectId} not found.");
                return Result<IEnumerable<TaskDto>>.Failure($"Project with id: {projectId} not found.");
            }

            var tasks = await _repositoryManager.Task.GetTasksByStatusAsync(
                projectId,
                Enum.Parse<TaskStatus>(status ?? "Pending"),
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo($"Tasks in project {project.Name} with status {status} are retrieved successfuly.");
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);     
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving tasks by status: {status} for project id: {projectId}. Error: {ex.Message}");
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
                _logger.LogWarn($"Project with id: {projectId} not found.");
                return Result<IEnumerable<TaskDto>>.Failure($"Project with id: {projectId} not found.");
            }

            var tasks = await _repositoryManager.Task.GetTasksByPriorityAsync(
                projectId,
                Enum.Parse<TaskPriority>(priority ?? "Urgent"),
                cancellationToken);

            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            _logger.LogInfo($"Tasks in project {project.Name} with priority {priority} are retrieved successfuly.");
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);     
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving tasks by priority: {priority} for project id: {projectId}. Error: {ex.Message}");
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
                _logger.LogWarn($"Parent task with id: {parentTaskId} not found.");
                return Result<IEnumerable<TaskDto>>.Failure($"Parent task with id: {parentTaskId} not found.");
            }    

            var subtasks = await _repositoryManager.Task.GetSubtasksAsync(
                parentTaskId,
                cancellationToken);

            var subtaskDtos = _mapper.Map<IEnumerable<TaskDto>>(subtasks);

            _logger.LogInfo($"Retrieved subtasks for parent task id: {parentTaskId} successfully.");
            return Result<IEnumerable<TaskDto>>.Success(subtaskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving subtasks for parent task id: {parentTaskId}. Error: {ex.Message}");
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

        _logger.LogInfo($"Created task with id: {task.Id} successfully.");
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
            _logger.LogWarn($"Task with id: {taskId} not found.");
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

        _logger.LogInfo($"Updated task with id: {taskId} successfully.");
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
            _logger.LogWarn($"Task with id: {taskId} not found.");
            return Result.Failure($"Task with id: {taskId} not found.");
        }

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;

        await _repositoryManager.SaveAsync(cancellationToken);

        _logger.LogInfo($"Deleted task with id: {taskId} successfully.");
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
            _logger.LogWarn($"Task with id: {taskId} not found.");
            return Result.Failure($"Task with id: {taskId} not found.");
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
            _logger.LogWarn($"Parent task with id: {parentTaskId} not found.");
            return Result<TaskDetailsDto>.Failure($"Parent task with id: {parentTaskId} not found.");
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

        _logger.LogInfo($"Added subtask with id: {subtask.Id} to parent task id: {parentTaskId} successfully.");
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
            _logger.LogWarn($"Task with id: {taskId} not found.");
            return Result<TaskDetailsDto>.Failure($"Task with id: {taskId} not found.");
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

        _logger.LogInfo($"Added attachment with id: {attachment.Id} to task id: {taskId} successfully.");
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
            _logger.LogWarn($"Task with id: {taskId} not found.");
            return Result.Failure($"Task with id: {taskId} not found.");
        }

        var attachment = await _repositoryManager.Attachment.GetByIdAsync(
            attachmentId,
            cancellationToken);

        if (attachment is null || attachment.TaskId != taskId)
        {
            _logger.LogWarn($"Attachment with id: {attachmentId} not found for task id: {taskId}.");
            return Result.Failure($"Attachment with id: {attachmentId} not found for task id: {taskId}.");
        }

         _repositoryManager.Attachment.Remove(
            attachment);
        await _repositoryManager.SaveAsync(cancellationToken);

        _logger.LogInfo($"Removed attachment with id: {attachmentId} from task id: {taskId} successfully.");
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
            _logger.LogWarn($"Task with id: {taskId} not found.");
            return Result<TaskDetailsDto>.Failure($"Task with id: {taskId} not found.");
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

        _logger.LogInfo($"Added comment with id: {comment.Id} to task id: {taskId} successfully.");
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
            _logger.LogWarn($"Task with id: {taskId} not found.");
            return Result.Failure($"Task with id: {taskId} not found.");
        }

        var comment = await _repositoryManager.Comment.GetByIdAsync(
            commentId,
            cancellationToken);

        if (comment is null || comment.TaskId != taskId)
        {
            _logger.LogWarn($"Comment with id: {commentId} not found for task id: {taskId}.");
            return Result.Failure($"Comment with id: {commentId} not found for task id: {taskId}.");
        }

         _repositoryManager.Comment.Remove(
            comment);
        await _repositoryManager.SaveAsync(cancellationToken);

        _logger.LogInfo($"Removed comment with id: {commentId} from task id: {taskId} successfully.");
        return Result.Success();
    }
}