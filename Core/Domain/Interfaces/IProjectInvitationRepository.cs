using System;
using Domain.Entities.Models;

namespace Domain.Interfaces;

public interface IProjectInvitationRepository : IRepository<ProjectInvitation>
{
    Task<ProjectInvitation?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default);
}
