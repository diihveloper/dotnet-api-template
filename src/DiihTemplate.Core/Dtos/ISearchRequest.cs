namespace DiihTemplate.Core.Dtos;

public interface ISearchRequest : IRequest
{
    string? Search { get; set; }
}