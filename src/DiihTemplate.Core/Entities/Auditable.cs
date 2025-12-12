namespace DiihTemplate.Core.Entities;

public abstract class Auditable<TKey> : Entity<TKey>, IAuditable
{
    protected Auditable()
    {
    }

    protected Auditable(TKey id) : base(id)
    {
    }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}