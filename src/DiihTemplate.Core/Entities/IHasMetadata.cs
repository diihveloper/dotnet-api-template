namespace DiihTemplate.Core.Entities;

public interface IHasMetadata
{
    Dictionary<string, string>? Metadata { get; set; }
}