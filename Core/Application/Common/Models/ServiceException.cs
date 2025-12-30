
namespace Application.Common.Models;

public class ServiceException : Exception
{
    public ServiceException(string message) : base(message)
    {
    }
    public ServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class NotFoundException : ServiceException
{
    public NotFoundException(string message) : base(message)
    {
    }
    public NotFoundException(string entityName, Guid id)
        : base($"{entityName} with ID {id} was not found.")
    {
    }
}

public class UnAuthorizedException : ServiceException
{
    public UnAuthorizedException(string message = "Unauthorized access") : base(message)
    {
    }
}

public class ValidationException : ServiceException
{
    public List<string> Errors { get; set; }

    public ValidationException(List<string> errors, string message = "Validation failed")
        : base(message)
    {
        Errors = errors;
    }

    public ValidationException(string message)
        : base(message)
    {
        Errors = [message];
    }
}