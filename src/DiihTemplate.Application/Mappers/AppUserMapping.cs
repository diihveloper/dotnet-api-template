using DiihTemplate.Application.Contracts;
using DiihTemplate.Domain;
using Mapster;

namespace DiihTemplate.Application.Mappers;

public class AppUserMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AppUser, AppUserDto>();
    }
}