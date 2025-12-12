using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DiihTemplate.Core.Services;

public class CurrentUserService<TUser> : ICurrentUserService<TUser> where TUser : class
{
    private UserManager<TUser> _userManager;
    private IHttpContextAccessor _httpContextAccessor;

    private TUser? _user;

    public CurrentUserService(UserManager<TUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TUser?> GetCurrentUserAsync()
    {
        _user ??= await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        return _user;
    }
}