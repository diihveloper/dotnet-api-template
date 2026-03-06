#if STORAGE
namespace DiihTemplate.Infra.Storage;

public class StorageSettings
{
    public const string SectionName = "Storage";

    public required string BasePath { get; set; }
}
#endif
