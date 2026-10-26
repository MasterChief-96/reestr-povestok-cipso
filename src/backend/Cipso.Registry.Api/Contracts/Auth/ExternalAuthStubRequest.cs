using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Contracts;

public sealed record ExternalAuthStubRequest(
    [Required(ErrorMessage = "Выберите способ входа.")] string Provider,
    Guid AccountId);
