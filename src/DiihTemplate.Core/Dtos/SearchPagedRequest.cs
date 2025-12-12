namespace DiihTemplate.Core.Dtos;

public class SearchPagedRequest : PagedRequest, ISearchRequest
{
    public string? Search { get; set; }
}