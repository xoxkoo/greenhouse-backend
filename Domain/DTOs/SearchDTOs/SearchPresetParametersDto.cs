namespace Domain.DTOs;

public class SearchPresetParametersDto
{
    public int? Id { get; }
    public bool? IsCurrent { get; }

    public SearchPresetParametersDto(int? id = null, bool? isCurrent = null)
    {
        Id = id;
        IsCurrent = isCurrent;
    }
}