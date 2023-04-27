using Domain.DTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IScheduleDao
{
    Task<ScheduleDto> CreateAsync(Schedule schedule);
}