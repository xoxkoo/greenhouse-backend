﻿using Domain.DTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IPresetDao
{
    Task<IEnumerable<PresetDto>> GetAsync(SearchPresetParametersDto parametersDto);
    
}