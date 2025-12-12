namespace DiihTemplate.Core.Dtos;

public interface IPagedRequest : IRequest
{
    int Page { get; set; }
    int PageSize { get; set; }

    IPagedRequest Validate();
    IPagedRequest Validate(int PageSize);
}