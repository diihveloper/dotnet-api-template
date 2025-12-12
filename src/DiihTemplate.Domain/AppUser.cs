using DiihTemplate.Core.Attributes;
using DiihTemplate.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace DiihTemplate.Domain;

public class AppUser : IdentityUser, IEntity<string>
{
    [Searchable] public string Nome { get; set; } = string.Empty;

    [ProtectedPersonalData][Searchable] public override string? Email { get; set; }

    public virtual ICollection<IdentityRole> Roles { get; set; } = [];

    public object[] GetKeys()
    {
        return [Id];
    }
}