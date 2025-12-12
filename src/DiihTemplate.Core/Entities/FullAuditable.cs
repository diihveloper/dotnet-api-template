namespace DiihTemplate.Core.Entities;

public abstract class FullAuditable<TKey, TUserKey> : Auditable<TKey>, IFullAuditable<TUserKey>
{
    protected FullAuditable()
    {
    }

    protected FullAuditable(TKey id) : base(id)
    {
    }

    public TUserKey? CreatorId { get; set; }
    public TUserKey? UpdaterId { get; set; }
    public TUserKey? DeleterId { get; set; }
}

public abstract class FullAuditable<TKey, TUserKey, TUser> : FullAuditable<TKey, TUserKey>,
    IFullAuditable<TUserKey, TUser>
{
    public virtual TUser? Creator { get; set; }
    public virtual TUser? Updater { get; set; }
    public virtual TUser? Deleter { get; set; }
}