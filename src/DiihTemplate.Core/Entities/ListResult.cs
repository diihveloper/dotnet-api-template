using DiihTemplate.Core.Commons;

namespace DiihTemplate.Core.Entities;

public class ListResult<T> : IListResult<T>
{
    public static implicit operator ListResult<T>(List<T> v)
    {
        return new ListResult<T>(v);
    }

    public static implicit operator List<T>(ListResult<T> v)
    {
        return v.Items.ToList();
    }

    public IReadOnlyList<T> Items { get; set; }

    public ListResult()
    {
        Items = new List<T>();
    }

    public ListResult(IReadOnlyList<T> items)
    {
        Items = items;
    }
}