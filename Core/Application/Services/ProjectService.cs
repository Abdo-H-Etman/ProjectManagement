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
    public ProjectService(IRepositoryManager repositoryManager, ILoggerManager logger, ICurrentUserService currentUserService, IEmailService emailService)
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

            return Result<ProjectListDto>.Success(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetProjectByIdAsync: {ex.Message}");
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

            return Result<IEnumerable<ProjectListDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetAllProjectsAsync: {ex.Message}");
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
                return Result<ProjectDetailsDto>.Failure("Project not found.");
            }

            var currentUserId = _currentUserService.UserId;
            var isProjectMember = await IsUserProjectMemberAsync(projectId, currentUserId, cancellationToken);
            if (!isProjectMember.IsSuccess)
            {
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

            return Result<ProjectDetailsDto>.Success(projectDetailsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetProjectDetailsByIdAsync: {ex.Message}");
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

            return Result<IEnumerable<ProjectListDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetUserProjectsAsync: {ex.Message}");
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
                return Result<ProjectStatistics>.Failure("Project not found.");
            }

            var projectStatistics = await _repositoryManager.Project.GetProjectStatisticsAsync(projectId, cancellationToken);


            return Result<ProjectStatistics>.Success(projectStatistics);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetProjectStatisticsAsync: {ex.Message}");
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

            return Result<ProjectListDto>.Success(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in UpdateProjectAsync: {ex.Message}");
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
                return Result.Failure("Project not found.");
            }

            _repositoryManager.Project.SoftDelete(project);
            await _repositoryManager.SaveAsync(cancellationToken);

            return Result.Success("Project deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in DeleteProjectAsync: {ex.Message}");
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

            return Result<IEnumerable<ProjectListDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetArchivedProjectsAsync: {ex.Message}");
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

            return Result<ProjectListDto>.Success(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CreateProjectAsync: {ex.Message}");
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

            return Result<IEnumerable<ProjectListDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SearchProjectsAsync: {ex.Message}");
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
            _logger.LogError($"Error in IsUserProjectMemberAsync: {ex.Message}");
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

            return Result.Success("Invitation sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SendProjectInvitationAsync: {ex.Message}");
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

            return Result.Success("Invitation accepted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in AcceptProjectInvitationAsync: {ex.Message}");
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
                return Result.Failure("Project not found.");
            }

            project.IsArchived = true;
            _repositoryManager.Project.Update(project);
            await _repositoryManager.SaveAsync(cancellationToken);

            return Result.Success("Project archived successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ArchiveProjectAsync: {ex.Message}");
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

            return Result.Success("Projects archived successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ArchiveProjects: {ex.Message}");
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
                return Result.Failure("Project not found.");
            }

            project.IsArchived = false;
            _repositoryManager.Project.Update(project);
            await _repositoryManager.SaveAsync(cancellationToken);

            return Result.Success("Project restored successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in RestoreProjectAsync: {ex.Message}");
            return Result.Failure("An error occurred while restoring the project.");
        }
    }
}
