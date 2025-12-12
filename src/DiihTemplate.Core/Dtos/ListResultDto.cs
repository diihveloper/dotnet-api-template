using DiihTemplate.Core.Commons;

namespace DiihTemplate.Core.Dtos;

public class ListResultDto<T> : IListResult<T>
{
    public static implicit operator ListResultDto<T>(List<T> v)
    {
        return new ListResultDto<T>(v);
    }

    public static implicit operator List<T>(ListResultDto<T> v)
    {
        return v.Items.ToList();
    }

    public IReadOnlyList<T> Items { get; set; }

    public ListResultDto()
    {
        Items = new List<T>();
    }

    public ListResultDto(IReadOnlyList<T> items)
    {
        Items = items;
    }
}