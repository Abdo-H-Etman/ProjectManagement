
namespace Application.Common.Models.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
