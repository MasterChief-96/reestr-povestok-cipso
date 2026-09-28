using System.ComponentModel.DataAnnotations;
using Cipso.Registry.Api.Domain;

namespace Cipso.Registry.Api.Contracts;

public sealed record AddressRequest(
    string PostalCode,
    string Region,
    string City,
    string Street,
    string Building,
    string? Apartment);

public sealed record CreateCitizenRequest(
    [Required] string RegistryNumber,
    [Required] string LastName,
    [Required] string FirstName,
    string? MiddleName,
    DateOnly BirthDate,
    string? Email,
    string? Phone,
    [Required] AddressRequest Address);

public sealed record CreateSummonsRequest(
    [Required] string Number,
    Guid CitizenId,
    Guid AuthorityOfficeId,
    Guid CreatedByEmployeeId,
    DateOnly IssuedAt,
    DateTimeOffset DueAt,
    [Required] string Reason,
    string? Comment);

public sealed record ChangeStatusRequest(
    SummonsStatus Status,
    [Required] string Actor,
    string? Comment);

public sealed record CreateNotificationRequest(
    NotificationChannel Channel,
    [Required] string Destination);

public sealed record CreateAppealRequest(
    [Required] string Type,
    [Required] string Text,
    [Required] string Actor);
