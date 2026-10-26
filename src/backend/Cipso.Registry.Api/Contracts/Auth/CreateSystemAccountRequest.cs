using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Contracts;

public sealed record CreateSystemAccountRequest(
    [Required(ErrorMessage = "Укажите ФИО или отображаемое имя.")] string DisplayName,
    [Required(ErrorMessage = "Выберите роль пользователя.")] string Role,
    CreateCitizenRequest? Citizen);
