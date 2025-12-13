using DiihTemplate.Core.Dtos;

namespace DiihTemplate.Application.Contracts;

public class AppUserDto : EntityDto<string>
{
    public string Name { get; set; }
    public string Email { get; set; }
}