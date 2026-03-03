using System;
using Application.Common.Models;
using Application.Common.Models.Interfaces;
using Application.DTOs;
using Application.DTOs.Attachment;
using Application.DTOs.Project;
using Application.DTOs.Task;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Interfaces.Logging;
using Application.Interfaces.Mailing;
using Domain.Entities.Enums;
using Domain.Entities.Models;
using Domain.Interfaces;

namespace Application.Services;

public class ProjectService : IProjectService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;
    public ProjectService(IRepositoryManager repositoryManager,ILoggerManager logger,
                          ICurrentUserService currentUserService, IEmailService emailService)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<Result<ProjectListDto>> GetProjectByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetProjectWithDetailsAsync(projectId, cancellationToken);
            if (project == null)
            {
                _logger.LogWarn("Project with ID {projectId} not found.", projectId);
                return Result<ProjectListDto>.Failure("Project not found.");
            }

            var projectDto = new ProjectListDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Visibility = project.Visibility.ToString(),
                IsArchived = project.IsArchived,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                IsDeleted = project.IsDeleted,
                DeletedAt = project.DeletedAt,
                Owner = new UserDto
                {
                    Id = project.Owner.Id,
                    FullName = $"{project.Owner.FirstName} {project.Owner.LastName}",
                    Email = project.Owner.Email!,
                    UserName = project.Owner.UserName!
                },
            };
            _logger.LogInfo("Retrieved project with ID {projectId}", projectId);
            return Result<ProjectListDto>.Success(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetProjectByIdAsync: {message}", ex.Message);
            return Result<ProjectListDto>.Failure($"An error occurred while retrieving the project");
        }
    }

    public async Task<Result<IEnumerable<ProjectListDto>>> GetAllProjectsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var projects = await _repositoryManager.Project.GetAllAsync(cancellationToken);
            var projectDtos = projects.Select(project => new ProjectListDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Visibility = project.Visibility.ToString(),
                IsArchived = project.IsArchived,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                IsDeleted = project.IsDeleted,
                DeletedAt = project.DeletedAt,
                Owner = new UserDto
                {
                    Id = project.Owner.Id,
                    FullName = $"{project.Owner.FirstName} {project.Owner.LastName}",
                    Email = project.Owner.Email!,
                    UserName = project.Owner.UserName!
                },
            });
            _logger.LogInfo("User with ID {userId} retrieved {count} projects", _currentUserService.UserId, projectDtos.Count());
            return Result<IEnumerable<ProjectListDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetAllProjectsAsync: {message}", ex.Message);
            return Result<IEnumerable<ProjectListDto>>.Failure($"An error occurred while retrieving projects");
        }
    }

    public async Task<Result<ProjectDetailsDto>> GetProjectDetailsByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetProjectWithDetailsAsync(projectId, cancellationToken);
            if (project == null)
            {
                _logger.LogWarn("Project with ID {projectId} not found.", projectId);
                return Result<ProjectDetailsDto>.Failure("Project not found.");
            }

            var currentUserId = _currentUserService.UserId;
            var isProjectMember = await IsUserProjectMemberAsync(projectId, currentUserId, cancellationToken);
            if (!isProjectMember.IsSuccess)
            {
                _logger.LogError("Error User with ID {userId} is not a member of project ID {projectId}",
                    currentUserId, projectId);
                return Result<ProjectDetailsDto>.Failure("Access denied. User is not a member of the project.");
            }

            var projectDetailsDto = new ProjectDetailsDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Visibility = project.Visibility.ToString(),
                IsArchived = project.IsArchived,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                IsDeleted = project.IsDeleted,
                DeletedAt = project.DeletedAt,
                Owner = new UserDto
                {
                    Id = project.Owner.Id,
                    FullName = $"{project.Owner.FirstName} {project.Owner.LastName}",
                    Email = project.Owner.Email!,
                    UserName = project.Owner.UserName!
                },
                Tasks = [.. project.Tasks
                    .Select(t => new TaskSummaryDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Status = t.Status.ToString(),
                    })],
                Members = [.. project.Members
                    .Where(pm => pm.IsActive)
                    .Select(pm => new UserDto
                    {
                        Id = pm.User!.Id,
                        FullName = $"{pm.User.FirstName} {pm.User.LastName}",
                        Email = pm.User.Email!,
                        UserName = pm.User.UserName!
                    })],
                Tags = [.. project.ProjectTags.Select(pt => pt.Tag.Name)],
                Attachments = [.. project.Attachments
                    .Select(a => new AttachmentDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FileUrl = a.FileUrl,
                        UploadedAt = a.CreatedAt,
                    })]
            };

            _logger.LogInfo("Retrieved details for project ID {projectId}", projectId);
            return Result<ProjectDetailsDto>.Success(projectDetailsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetProjectDetailsByIdAsync: {message}", ex.Message);
            return Result<ProjectDetailsDto>.Failure($"An error occurred while retrieving the project details");
        }
    }

    public async Task<Result<IEnumerable<ProjectListDto>>> GetUserProjectsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repositoryManager.User.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return Result<IEnumerable<ProjectListDto>>.Failure("User not found.");
            }

            var projects = await _repositoryManager.Project.GetUserProjectsAsync(userId, cancellationToken);
            var projectDtos = projects.Select(project => new ProjectListDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Visibility = project.Visibility.ToString(),
                IsArchived = project.IsArchived,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                IsDeleted = project.IsDeleted,
                DeletedAt = project.DeletedAt,
                Owner = new UserDto
                {
                    Id = project.Owner.Id,
                    FullName = $"{project.Owner.FirstName} {project.Owner.LastName}",
                    Email = project.Owner.Email!,
                    UserName = project.Owner.UserName!
                },
            });
            _logger.LogInfo("Retrieved {count} projects for user ID {userId}", projectDtos.Count(), userId);
            return Result<IEnumerable<ProjectListDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetUserProjectsAsync: {message}", ex.Message);
            return Result<IEnumerable<ProjectListDto>>.Failure($"An error occurred while retrieving user projects");
        }
    }

    public async Task<Result<ProjectStatistics>> GetProjectStatisticsAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                _logger.LogWarn("Project with ID {projectId} not found.", projectId);
                return Result<ProjectStatistics>.Failure("Project not found.");
            }

            var projectStatistics = await _repositoryManager.Project.GetProjectStatisticsAsync(projectId, cancellationToken);

            _logger.LogInfo("Retrieved statistics for project ID {@projectId}", projectId);
            return Result<ProjectStatistics>.Success(projectStatistics);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetProjectStatisticsAsync: {message}", ex.Message);
            return Result<ProjectStatistics>.Failure($"An error occurred while retrieving project statistics");
        }
    }

    public async Task<Result<ProjectListDto>> UpdateProjectAsync(
        Guid projectId,
        UpdateProjectDto updateProjectDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                _logger.LogWarn("Project with ID {projectId} not found.", projectId);
                return Result<ProjectListDto>.Failure("Project not found.");
            }

            project.Name = updateProjectDto.Name ?? project.Name;
            project.Description = updateProjectDto.Description ?? project.Description;
            project.Visibility = Enum.Parse<Visibility>(updateProjectDto.Visibility ?? project.Visibility.ToString());
            project.StartDate = updateProjectDto.StartDate ?? project.StartDate;
            project.EndDate = updateProjectDto.EndDate;

            _repositoryManager.Project.Update(project);
            await _repositoryManager.SaveAsync(cancellationToken);

            var projectDto = new ProjectListDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Visibility = project.Visibility.ToString(),
                IsArchived = project.IsArchived,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                IsDeleted = project.IsDeleted,
                DeletedAt = project.DeletedAt,
                Owner = new UserDto
                {
                    Id = project.Owner.Id,
                    FullName = $"{project.Owner.FirstName} {project.Owner.LastName}",
                    Email = project.Owner.Email!,
                    UserName = project.Owner.UserName!
                },
            };

            _logger.LogInfo("Updated project with ID {@projectId}", projectId);
            return Result<ProjectListDto>.Success(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in UpdateProjectAsync: {message}", ex.Message);
            return Result<ProjectListDto>.Failure($"An error occurred while updating the project");
        }
    }

    public async Task<Result> DeleteProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                _logger.LogWarn("Project with ID {projectId} not found.", projectId);
                return Result.Failure("Project not found.");
            }

            _repositoryManager.Project.SoftDelete(project);
            await _repositoryManager.SaveAsync(cancellationToken);

            _logger.LogInfo("Deleted project with ID {@projectId}", projectId);
            return Result.Success("Project deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in DeleteProjectAsync: {message}", ex.Message);
            return Result.Failure("An error occurred while deleting the project.");
        }
    }

    public async Task<Result<IEnumerable<ProjectListDto>>> GetArchivedProjectsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repositoryManager.User.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return Result<IEnumerable<ProjectListDto>>.Failure("User not found.");
            }

            var projects = await _repositoryManager.Project.GetArchivedProjectsAsync(userId, cancellationToken);
            var projectDtos = projects.Select(project => new ProjectListDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Visibility = project.Visibility.ToString(),
                IsArchived = project.IsArchived,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                IsDeleted = project.IsDeleted,
                DeletedAt = project.DeletedAt,
                Owner = new UserDto
                {
                    Id = project.Owner.Id,
                    FullName = $"{project.Owner.FirstName} {project.Owner.LastName}",
                    Email = project.Owner.Email!,
                    UserName = project.Owner.UserName!
                },
            });

            _logger.LogInfo("Retrieved {count} archived projects for user ID {userId}", projectDtos.Count(), userId);
            return Result<IEnumerable<ProjectListDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetArchivedProjectsAsync: {message}", ex.Message);
            return Result<IEnumerable<ProjectListDto>>.Failure($"An error occurred while retrieving archived projects");
        }
    }

    public async Task<Result<ProjectListDto>> CreateProjectAsync(
        CreateProjectDto createProjectDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ownerId = _currentUserService.UserId;
            var owner = await _repositoryManager.User.GetByIdAsync(ownerId, cancellationToken);
            if (owner == null)
            {
                return Result<ProjectListDto>.Failure("Owner user not found.");
            }

            var project = new Project
            {
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                Visibility = Enum.Parse<Visibility>(createProjectDto.Visibility),
                StartDate = createProjectDto.StartDate,
                EndDate = createProjectDto.EndDate,
                OwnerId = ownerId,
            };

            await _repositoryManager.Project.AddAsync(project, cancellationToken);

            await _repositoryManager.ProjectMember.AddProjectMemberAsync(new ProjectMember
            {
                ProjectId = project.Id,
                UserId = ownerId,
                Role = MemberRole.Owner,
                IsActive = true,
                JoinedAt = DateTime.UtcNow
            }, cancellationToken);

            await _repositoryManager.SaveAsync(cancellationToken);

            var projectDto = new ProjectListDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Visibility = project.Visibility.ToString(),
                IsArchived = project.IsArchived,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                IsDeleted = project.IsDeleted,
                DeletedAt = project.DeletedAt,
                Owner = new UserDto
                {
                    Id = owner.Id,
                    FullName = $"{owner.FirstName} {owner.LastName}",
                    Email = owner.Email!,
                    UserName = owner.UserName!
                },
            };

            _logger.LogInfo("Project created successfully: {name} (ID: {Id}) by User ID: {ownerId}",
                project.Name, project.Id, ownerId);
            return Result<ProjectListDto>.Success(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in CreateProjectAsync: {message}", ex.Message);
            return Result<ProjectListDto>.Failure($"An error occurred while creating the project {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<ProjectListDto>>> SearchProjectsAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _currentUserService.UserId;
            var projects = await _repositoryManager.Project.SearchProjectsAsync(searchTerm, userId, cancellationToken);
            var projectDtos = projects.Select(project => new ProjectListDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Visibility = project.Visibility.ToString(),
                IsArchived = project.IsArchived,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                IsDeleted = project.IsDeleted,
                DeletedAt = project.DeletedAt,
                Owner = new UserDto
                {
                    Id = project.Owner.Id,
                    FullName = $"{project.Owner.FirstName} {project.Owner.LastName}",
                    Email = project.Owner.Email!,
                    UserName = project.Owner.UserName!
                },
            });

            _logger.LogInfo("User with ID {userId} searched for projects with term '{searchTerm}' and found {count} results",
                userId, searchTerm, projectDtos.Count());
            return Result<IEnumerable<ProjectListDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in SearchProjectsAsync: {message}", ex.Message);
            return Result<IEnumerable<ProjectListDto>>.Failure($"An error occurred while searching projects");
        }
    }

    public async Task<Result<bool>> IsUserProjectMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var isMember = await _repositoryManager.Project.IsUserProjectMemberAsync(projectId, userId, cancellationToken);
            return Result<bool>.Success(isMember);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in IsUserProjectMemberAsync: {message}", ex.Message);
            return Result<bool>.Failure("An error occurred while checking project membership.");
        }
    }

    public async Task<Result> SendProjectInvitationAsync(
        Guid projectId,
        InvitationDto invitationDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = _repositoryManager.Project.GetByIdAsync(projectId, cancellationToken).Result;
            if (project == null)
            {
                _logger.LogWarn("Project with ID {projectId} not found.", projectId);
                return Result.Failure("Project not found.") ;
            }

            var inviterName = _currentUserService.UserName;
            var inviterId = _currentUserService.UserId;

            var invitation = new ProjectInvitation
            {
                ProjectId = projectId,
                InvitedById = inviterId,
                Email = invitationDto.Email,
                Role = Enum.Parse<MemberRole>(invitationDto.Role),
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };  
            await _repositoryManager.ProjectInvitation.AddAsync(invitation, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);

            var invitationLink = $"https://frontend.com.com/invitations/accept?token={invitation.Token}$inviterId={inviterId}";

            await _emailService.SendProjectInvitationAsync(
                invitationDto.Email,
                project.Name,
                inviterName!,
                invitationLink,
                cancellationToken);

            _logger.LogInfo("Project invitation sent to {email} for project ID {projectId} by user ID {inviterId}",
                invitationDto.Email, projectId, inviterId);
            return Result.Success("Invitation sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in SendProjectInvitationAsync: {message}", ex.Message);
            return Result.Failure($"An error occurred while sending the project invitation. {ex.Message}");
        }
    }
    public async Task<Result> AcceptProjectInvitationAsync(
        string token,
        Guid inviterId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var invitation = await _repositoryManager.ProjectInvitation.GetByTokenAsync(token, cancellationToken);
            if (invitation == null || invitation.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarn("Invalid or expired invitation token");
                return Result.Failure("Invalid or expired invitation token.");
            }

            var userId = _currentUserService.UserId;

            var projectMember = new ProjectMember
            {
                InvitedById = inviterId,
                ProjectId = invitation.ProjectId,
                UserId = userId,
                Role = invitation.Role,
                IsActive = true,
                JoinedAt = DateTime.UtcNow
            };

            await _repositoryManager.ProjectMember.AddProjectMemberAsync(projectMember, cancellationToken);

            invitation.IsAccepted = true;
            _repositoryManager.ProjectInvitation.Update(invitation);

            await _repositoryManager.SaveAsync(cancellationToken);

            _logger.LogInfo("User with ID {userId} accepted invitation to project ID {projectId} from inviter ID {inviterId}",
                userId, invitation.ProjectId, inviterId);
            return Result.Success("Invitation accepted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in AcceptProjectInvitationAsync: {message}", ex.Message);
            return Result.Failure($"An error occurred while accepting the project invitation. {ex.Message}");
        }
    }

    public async Task<Result> ArchiveProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                _logger.LogWarn("Project with ID {projectId} not found.", projectId);
                return Result.Failure("Project not found.");
            }

            project.IsArchived = true;
            _repositoryManager.Project.Update(project);
            await _repositoryManager.SaveAsync(cancellationToken);

            _logger.LogInfo("Project with ID {projectId} archived successfully.", projectId);
            return Result.Success("Project archived successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in ArchiveProjectAsync: {message}", ex.Message);
            return Result.Failure("An error occurred while archiving the project.");
        }
    }

    public async Task<Result> ArchiveMultipleProjectsAsync(
        List<Guid> projectIds,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var projects = await _repositoryManager.Project.FindAsync(
                p => projectIds.Contains(p.Id), cancellationToken
            );
            foreach (var project in projects)
            {
                project.IsArchived = true;
            }
            _repositoryManager.Project.UpdateRange(projects);
            await _repositoryManager.SaveAsync(cancellationToken);

            _logger.LogInfo("Archived {count} projects successfully.", projects.Count());
            return Result.Success("Projects archived successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in ArchiveProjects: {message}", ex.Message);
            return Result.Failure("An error occurred while archiving the projects.");
        }
    }
    public async Task<Result> RestoreProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _repositoryManager.Project.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                _logger.LogWarn("Project with ID {projectId} not found.", projectId);
                return Result.Failure("Project not found.");
            }

            project.IsArchived = false;
            _repositoryManager.Project.Update(project);
            await _repositoryManager.SaveAsync(cancellationToken);

            _logger.LogInfo("Project with ID {projectId} restored successfully.", projectId);
            return Result.Success("Project restored successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in RestoreProjectAsync: {message}", ex.Message);
            return Result.Failure("An error occurred while restoring the project.");
        }
    }
}
