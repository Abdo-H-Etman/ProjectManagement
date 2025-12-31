
using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(up => up.UserId == userId, cancellationToken);
    }
}
