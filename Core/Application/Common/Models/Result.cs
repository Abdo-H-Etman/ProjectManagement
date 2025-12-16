
namespace Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];

    public static Result Success(string message = "Operation successful") =>
        new() { IsSuccess = true, Message = message };

    public static Result Failure(string error, string message = "Operation failed") =>
        new() { IsSuccess = false, Message = message, Errors = [error] };

    public static Result Failure(List<string> errors, string message = "Operation failed") =>
        new() { IsSuccess = false, Message = message, Errors = errors };    
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data, string message = "Operation successful") =>
        new() { IsSuccess = true, Message = message, Data = data };

    public new static Result<T> Failure(string error, string message = "Operation failed") =>
        new() { IsSuccess = false, Message = message, Errors = [error] };

    public new static Result<T> Failure(List<string> errors, string message = "Operation failed") =>
        new() { IsSuccess = false, Message = message, Errors = errors };
}