namespace TopGear.Application.DTOs.PartsDTO;

public class PagedPartsResponseDTO
{
    public IEnumerable<PartDTO> Items { get; set; } = [];

    public int Page { get; set; }

    public int Limit { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
