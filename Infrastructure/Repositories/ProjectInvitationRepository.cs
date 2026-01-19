using System;
using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProjectInvitationRepository : Repository<ProjectInvitation>, IProjectInvitationRepository
{
    public ProjectInvitationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProjectInvitation?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default) =>
        await _context.ProjectInvitations
            .FirstOrDefaultAsync(pi => pi.Token == token, cancellationToken);
}
