namespace DiihTemplate.Core.Dtos;

public class AuditableDto<TKey> : EntityDto<TKey>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}