using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuthorityOfficeId { get; set; }
    public AuthorityOffice AuthorityOffice { get; set; } = null!;
    [MaxLength(32)] public string PersonnelNumber { get; set; } = string.Empty;
    [MaxLength(180)] public string FullName { get; set; } = string.Empty;
    [MaxLength(64)] public string Role { get; set; } = "Operator";
    public List<Summons> CreatedSummonses { get; set; } = [];
}
