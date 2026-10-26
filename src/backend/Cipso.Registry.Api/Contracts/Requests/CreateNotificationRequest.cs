using System.ComponentModel.DataAnnotations;
using Cipso.Registry.Api.Domain;

namespace Cipso.Registry.Api.Contracts;

public sealed record CreateNotificationRequest(
    NotificationChannel Channel,
    [Required(ErrorMessage = "Укажите адрес назначения.")] string Destination);
