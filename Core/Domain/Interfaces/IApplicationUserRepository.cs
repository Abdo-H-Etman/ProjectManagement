using Domain.Entities.Models;

namespace Domain.Interfaces;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByIdWithProfileAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetUsersByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ApplicationUser> AddAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    void Update(ApplicationUser user);
    void Delete(ApplicationUser user);
}
