using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class AuthorityOffice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(32)] public string Code { get; set; } = string.Empty;
    [MaxLength(180)] public string Name { get; set; } = string.Empty;
    [MaxLength(120)] public string Region { get; set; } = string.Empty;
    public List<Employee> Employees { get; set; } = [];
    public List<Summons> Summonses { get; set; } = [];
}
