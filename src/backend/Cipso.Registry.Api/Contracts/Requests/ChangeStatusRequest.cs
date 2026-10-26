using System.ComponentModel.DataAnnotations;
using Cipso.Registry.Api.Domain;

namespace Cipso.Registry.Api.Contracts;

public sealed record ChangeStatusRequest(
    SummonsStatus Status,
    [Required(ErrorMessage = "Укажите автора изменения.")] string Actor,
    string? Comment);
