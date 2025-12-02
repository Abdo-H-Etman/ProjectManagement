using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ApplicationUserRepository : IApplicationUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    public ApplicationUserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<ApplicationUser?> GetByIdWithProfileAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Users
            .Include(u => u.UserProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _userManager.FindByEmailAsync(email);

    public async Task<ApplicationUser?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await _userManager.FindByNameAsync(username);

    public async Task<IEnumerable<ApplicationUser>> GetUsersByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) =>
        await _context.Users
            .AsNoTracking()
            .Where(u => ids.Contains(u.Id))
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Users.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<ApplicationUser> AddAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        await _userManager.CreateAsync(user);
        return user;
    }

    public void Update(ApplicationUser user) =>
        _context.Users.Update(user);

    public void Delete(ApplicationUser user) =>
        _context.Users.Remove(user);    
}
