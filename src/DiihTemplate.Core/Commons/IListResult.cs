namespace DiihTemplate.Core.Commons;

public interface IListResult<T>
{
    IReadOnlyList<T> Items { get; set; }
}