using Microsoft.AspNetCore.Mvc;

namespace Cipso.Registry.Api.Validation;

/// <summary>Один формат ошибок для клиента и сервера.</summary>
public static class ValidationResponses
{
    public static ObjectResult Field(string field, string message, int status = 400) =>
        new(new { message, errors = new Dictionary<string, string[]> { [field] = [message] } })
        { StatusCode = status };

    public static IActionResult ModelState(ActionContext context)
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => string.IsNullOrWhiteSpace(entry.Key) ? "form" : entry.Key,
                entry => entry.Value!.Errors.Select(e => Localize(e.ErrorMessage)).ToArray());

        return new BadRequestObjectResult(new
        {
            message = "Исправьте ошибки в указанных полях.",
            errors
        });
    }

    private static string Localize(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return "Проверьте значение поля.";
        if (message.Contains("required", StringComparison.OrdinalIgnoreCase))
            return "Поле обязательно для заполнения.";
        if (message.Contains("could not be converted", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("is not valid", StringComparison.OrdinalIgnoreCase))
            return "Указан некорректный формат значения.";
        return message;
    }
}
