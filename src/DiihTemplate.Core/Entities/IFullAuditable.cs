namespace DiihTemplate.Core.Entities;

public interface IFullAuditable<TUserKey> : IAuditable, IHasCreator<TUserKey>, IHasUpdater<TUserKey>,
    IHasDeleter<TUserKey>
{
}

public interface IFullAuditable<TUserKey, TUser> : IAuditable, IHasCreator<TUserKey, TUser>,
    IHasUpdater<TUserKey, TUser>,
    IHasDeleter<TUserKey, TUser>
{
}