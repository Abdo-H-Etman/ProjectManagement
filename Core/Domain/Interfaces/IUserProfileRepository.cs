
using Domain.Entities.Models;

namespace Domain.Interfaces;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
