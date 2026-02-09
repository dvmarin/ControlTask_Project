using AutoMapper;
using ControlTask.Application.DTOs;
using ControlTask.Domain.Entities;

namespace ControlTask.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Developer mappings
            CreateMap<Developer, DeveloperDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

            // Project mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.TotalTasks,
                    opt => opt.MapFrom(src => src.Tasks.Count))
                .ForMember(dest => dest.OpenTasks,
                    opt => opt.MapFrom(src => src.Tasks.Count(t => t.Status != "Completed")))
                .ForMember(dest => dest.CompletedTasks,
                    opt => opt.MapFrom(src => src.Tasks.Count(t => t.Status == "Completed")));

            // Task mappings
            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.ProjectName,
                    opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.AssigneeName,
                    opt => opt.MapFrom(src => src.Assignee.FirstName + " " + src.Assignee.LastName));

            CreateMap<CreateTaskDto, TaskItem>();
            CreateMap<UpdateTaskStatusDto, TaskItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
