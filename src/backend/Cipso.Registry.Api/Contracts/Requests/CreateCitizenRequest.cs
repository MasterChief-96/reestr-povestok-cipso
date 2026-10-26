using System.ComponentModel.DataAnnotations;
using Cipso.Registry.Api.Domain;

namespace Cipso.Registry.Api.Contracts;

public sealed record CreateCitizenRequest(
    [Required(ErrorMessage = "Укажите реестровый номер.")]
    [MaxLength(64, ErrorMessage = "Реестровый номер: не более 64 символов.")] string RegistryNumber,
    [Required(ErrorMessage = "Укажите фамилию.")]
    [MaxLength(100, ErrorMessage = "Фамилия: не более 100 символов.")] string LastName,
    [Required(ErrorMessage = "Укажите имя.")]
    [MaxLength(100, ErrorMessage = "Имя: не более 100 символов.")] string FirstName,
    [MaxLength(100, ErrorMessage = "Отчество: не более 100 символов.")] string? MiddleName,
    DateOnly BirthDate,
    [EmailAddress(ErrorMessage = "Укажите корректный e-mail.")]
    [MaxLength(255, ErrorMessage = "E-mail: не более 255 символов.")] string? Email,
    [MaxLength(32, ErrorMessage = "Телефон: не более 32 символов.")] string? Phone,
    [Required(ErrorMessage = "Заполните адрес регистрации.")] AddressRequest Address);
