using AutoMapper;
using Application.DTOs.Task;
using Task = Domain.Entities.Models.Task;
using TaskStatus = Domain.Entities.Enums.TaskStatus;
using Domain.Entities.Enums;
using Application.DTOs.User;
using Application.DTOs.Project;
using Application.DTOs;

namespace Application.Mapping;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<Task, TaskDto>()
            .ForMember(dest => dest.Priority,
                opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<TaskDto, Task>()
            .ForMember(dest => dest.Priority,
                opt => opt.MapFrom(src => Enum.Parse<TaskPriority>(src.Priority)))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => Enum.Parse<TaskStatus>(src.Status)))
            .ForMember(dest => dest.Id,
                opt => opt.Ignore());

        CreateMap<Task, TaskDetailsDto>()
            .ForMember(dest => dest.Priority,
                opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.CreatedBy != null
                    ? new UserDto
                    {
                        Id = src.CreatedBy.Id,
                        FullName = $"{src.CreatedBy.FirstName} {src.CreatedBy.LastName}",
                        Email = src.CreatedBy.Email!
                    }
                    : null))
            .ForMember(dest => dest.Project,
                opt => opt.MapFrom(src => src.Project != null
                    ? new ProjectSummaryDto
                    {
                        Id = src.Project.Id,
                        Name = src.Project.Name,
                        Description = src.Project.Description,
                        Status = src.Project.Status.ToString(),
                    }
                    : null));

        CreateMap<CreateTaskDto, Task>()
            .ForMember(dest => dest.Priority,
                opt => opt.MapFrom(src => Enum.Parse<TaskPriority>(src.Priority)))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => Enum.Parse<TaskStatus>(src.Status)))
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,
                opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.Ignore());

        CreateMap<UpdateTaskDto, Task>()
            .ForMember(dest => dest.Priority,
                opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.Priority)
                ? Enum.Parse<TaskPriority>(src.Priority)
                : dest.Priority))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.Status)
                ? Enum.Parse<TaskStatus>(src.Status)
                : dest.Status))
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,
                opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Task, TaskSummaryDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));
    }
}