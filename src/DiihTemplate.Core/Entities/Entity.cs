namespace DiihTemplate.Core.Entities;

public interface IEntity<T> : IEntity
{
    T Id { get; }
}

public abstract class Entity : IEntity
{
    public abstract object[] GetKeys();
}

public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    public TKey Id { get; protected set; }

    protected Entity()
    {
    }

    protected Entity(TKey id)
    {
        Id = id;
    }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}