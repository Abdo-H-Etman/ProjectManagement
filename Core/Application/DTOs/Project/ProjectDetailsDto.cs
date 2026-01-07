using Application.DTOs.Attachment;
using Application.DTOs.Task;
using Application.DTOs.User;
using Domain.Entities.Models;

namespace Application.DTOs.Project;

public record ProjectDetailsDto : ProjectListDto
{
    public IEnumerable<TaskSummaryDto> Tasks { get; init; } = [];
    public IEnumerable<UserDto> Members { get; init; } = [];
    public IEnumerable<string> Tags { get; init; } = [];
    public IEnumerable<AttachmentDto> Attachments { get; init; } = [];
}
