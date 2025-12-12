namespace DiihTemplate.Core.Dtos;

public class SearchRequest : ISearchRequest
{
    public static implicit operator SearchPagedRequest(SearchRequest v)
    {
        return new SearchPagedRequest
        {
            Search = v.Search
        };
    }

    public static explicit operator SearchRequest(SearchPagedRequest v)
    {
        return new SearchRequest
        {
            Search = v.Search
        };
    }

    public string? Search { get; set; }
}