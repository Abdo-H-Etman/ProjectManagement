using System;
using Application.Common.Models;
using Application.DTOs;
using Application.DTOs.Project;
using Domain.Entities.Enums;
using Domain.Interfaces;

namespace Application.Interfaces;

public interface IProjectService
{
    Task<Result<ProjectListDto>> GetProjectByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<Result<ProjectDetailsDto>> GetProjectDetailsByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);    
    Task<Result<IEnumerable<ProjectListDto>>> GetAllProjectsAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProjectListDto>>> GetUserProjectsAsync(
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProjectListDto>>> GetArchivedProjectsAsync(
        CancellationToken cancellationToken = default);
    Task<Result<ProjectListDto>> CreateProjectAsync(
        CreateProjectDto createProjectDto,
        CancellationToken cancellationToken = default);
    Task<Result<ProjectListDto>> UpdateProjectAsync(
        Guid projectId,
        UpdateProjectDto updateProjectDto,
        CancellationToken cancellationToken = default);    
    Task<Result> DeleteProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProjectListDto>>> SearchProjectsAsync(
        string searchTerm,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> IsUserProjectMemberAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<Result<ProjectStatistics>> GetProjectStatisticsAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);       
    Task<Result> SendProjectInvitationAsync(
        Guid projectId,
        InvitationDto invitationDto,
        CancellationToken cancellationToken = default);
    Task<Result> AcceptProjectInvitationAsync(
        string token,
        CancellationToken cancellationToken = default);    
    Task<Result> ArchiveProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<Result> ArchiveMultipleProjectsAsync(
        List<Guid> projectIds,
        CancellationToken cancellationToken = default);    
    Task<Result> RestoreProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);        
}
