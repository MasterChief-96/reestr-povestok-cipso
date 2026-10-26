using System.ComponentModel.DataAnnotations;
using Cipso.Registry.Api.Domain;

namespace Cipso.Registry.Api.Contracts;

public sealed record CreateSummonsRequest(
    [Required(ErrorMessage = "Укажите номер повестки.")]
    [MaxLength(64, ErrorMessage = "Номер: не более 64 символов.")] string Number,
    Guid CitizenId,
    Guid AuthorityOfficeId,
    Guid CreatedByEmployeeId,
    DateOnly IssuedAt,
    DateTimeOffset DueAt,
    [Required(ErrorMessage = "Укажите основание явки.")]
    [MaxLength(500, ErrorMessage = "Причина: не более 500 символов.")] string Reason,
    [MaxLength(1000, ErrorMessage = "Комментарий: не более 1000 символов.")] string? Comment);
