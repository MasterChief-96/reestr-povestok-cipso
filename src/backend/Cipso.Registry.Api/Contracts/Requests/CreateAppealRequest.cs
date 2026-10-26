using System.ComponentModel.DataAnnotations;
using Cipso.Registry.Api.Domain;

namespace Cipso.Registry.Api.Contracts;

public sealed record CreateAppealRequest(
    [Required(ErrorMessage = "Укажите тип обращения.")] string Type,
    [Required(ErrorMessage = "Введите текст обращения.")] string Text,
    [Required(ErrorMessage = "Укажите автора обращения.")] string Actor);
