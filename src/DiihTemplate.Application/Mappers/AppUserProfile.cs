using AutoMapper;
using DiihTemplate.Application.Contracts;
using DiihTemplate.Domain;

namespace DiihTemplate.Application.Mappers;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUser, AppUserDto>();
    }
}