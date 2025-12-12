namespace DiihTemplate.Core.Services;

public interface ICurrentUserService<TUser> where TUser : class
{
    Task<TUser?> GetCurrentUserAsync();
}