using System;
using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProjectMemberRepository : IProjectMemberRepository
{
    private readonly ApplicationDbContext _context;
    public ProjectMemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectMember?> GetProjectMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _context.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId, cancellationToken);

    public async Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await _context.ProjectMembers
            .Where(pm => pm.ProjectId == projectId)
            .ToListAsync(cancellationToken);

    public async Task<ProjectMember> AddProjectMemberAsync(
        ProjectMember projectMember,
        CancellationToken cancellationToken = default)
    {
        await _context.ProjectMembers.AddAsync(projectMember,cancellationToken);
        return projectMember;
    }

    public void Update(ProjectMember projectMember)
    {
        _context.ProjectMembers.Update(projectMember);
    }

    public void Remove(ProjectMember projectMember)
    {
        _context.ProjectMembers.Remove(projectMember);
    }
            
}
