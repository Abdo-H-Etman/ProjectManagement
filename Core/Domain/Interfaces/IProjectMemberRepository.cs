using System;
using Domain.Entities.Models;

namespace Domain.Interfaces;

public interface IProjectMemberRepository
{
    Task<ProjectMember?> GetProjectMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<ProjectMember> AddProjectMemberAsync(
        ProjectMember projectMember,
        CancellationToken cancellationToken = default);
    void Update(ProjectMember projectMember);    
    void Remove(ProjectMember projectMember);            
}
