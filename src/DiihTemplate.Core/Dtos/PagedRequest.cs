namespace DiihTemplate.Core.Dtos;

public class PagedRequest : IPagedRequest
{
    private int _page;
    private int _pageSize;

    public string? OrderBy { get; set; }
    public string? Order { get; set; }

    public int Page
    {
        get => _page < 1 ? 1 : _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize < 1 ? 1 : _pageSize;
        set => _pageSize = value < 1 ? 1 : value;
    }

    public IPagedRequest Validate()
    {
        return Validate(1);
    }

    public IPagedRequest Validate(int pageSize)
    {
        if (Page < 1)
        {
            Page = 1;
        }

        if (PageSize < 1)
        {
            PageSize = pageSize;
        }

        return this;
    }
}