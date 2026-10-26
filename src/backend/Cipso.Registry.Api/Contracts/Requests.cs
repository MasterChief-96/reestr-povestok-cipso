using System.ComponentModel.DataAnnotations;
using Cipso.Registry.Api.Domain;

namespace Cipso.Registry.Api.Contracts;

public sealed record AddressRequest(
    [Required(ErrorMessage = "Укажите почтовый индекс.")]
    [MaxLength(16, ErrorMessage = "Индекс: не более 16 символов.")] string PostalCode,
    [Required(ErrorMessage = "Укажите регион.")]
    [MaxLength(120, ErrorMessage = "Регион: не более 120 символов.")] string Region,
    [Required(ErrorMessage = "Укажите город.")]
    [MaxLength(120, ErrorMessage = "Город: не более 120 символов.")] string City,
    [Required(ErrorMessage = "Укажите улицу.")]
    [MaxLength(120, ErrorMessage = "Улица: не более 120 символов.")] string Street,
    [Required(ErrorMessage = "Укажите номер дома.")]
    [MaxLength(32, ErrorMessage = "Дом: не более 32 символов.")] string Building,
    [MaxLength(32, ErrorMessage = "Квартира: не более 32 символов.")] string? Apartment);

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

public sealed record ChangeStatusRequest(
    SummonsStatus Status,
    [Required(ErrorMessage = "Укажите автора изменения.")] string Actor,
    string? Comment);

public sealed record CreateNotificationRequest(
    NotificationChannel Channel,
    [Required(ErrorMessage = "Укажите адрес назначения.")] string Destination);

public sealed record CreateAppealRequest(
    [Required(ErrorMessage = "Укажите тип обращения.")] string Type,
    [Required(ErrorMessage = "Введите текст обращения.")] string Text,
    [Required(ErrorMessage = "Укажите автора обращения.")] string Actor);
