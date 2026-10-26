namespace Cipso.Registry.Api.Validation;

/// <summary>Бизнес-ошибка конкретного поля формы.</summary>
public sealed class FieldValidationException(string field, string message, int statusCode = 400)
    : ArgumentException(message)
{
    public string Field { get; } = field;
    public int StatusCode { get; } = statusCode;
}
