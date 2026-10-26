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
