using System.Text;
using System.Text.Json;
using DevReview.API.Configuration;
using Ganss.Xss;
using Microsoft.Extensions.Options;

namespace DevReview.API.Middleware
{
    /// <summary>
    /// Sanitizes user-supplied text fields in JSON request bodies to mitigate stored XSS.
    /// </summary>
    public class InputSanitizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HtmlSanitizer _sanitizer;
        private readonly HashSet<string> _fieldNames;

        public InputSanitizationMiddleware(
            RequestDelegate next,
            IOptions<SanitizationOptions> options)
        {
            _next = next;
            _sanitizer = CreateSanitizer();
            _fieldNames = options.Value.Fields
                .Select(f => f.Trim())
                .Where(f => f.Length > 0)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ShouldSanitize(context))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        using var document = JsonDocument.Parse(body);
                        var sanitized = SanitizeElement(document.RootElement);
                        var newJson = JsonSerializer.Serialize(sanitized);
                        var bytes = Encoding.UTF8.GetBytes(newJson);
                        context.Request.Body = new MemoryStream(bytes);
                        context.Request.ContentLength = bytes.Length;
                    }
                    catch (JsonException)
                    {
                        context.Request.Body.Position = 0;
                    }
                }
            }

            await _next(context);
        }

        private static bool ShouldSanitize(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method) &&
                !HttpMethods.IsPut(context.Request.Method) &&
                !HttpMethods.IsPatch(context.Request.Method))
            {
                return false;
            }

            if (context.Request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) != true)
            {
                return false;
            }

            return true;
        }

        private object? SanitizeElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => SanitizeObject(element),
                JsonValueKind.Array => element.EnumerateArray().Select(SanitizeElement).ToList(),
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => null
            };
        }

        private static bool IsCodeFileObject(JsonElement element) =>
            element.TryGetProperty("fileName", out _) || element.TryGetProperty("FileName", out _);

        private Dictionary<string, object?> SanitizeObject(JsonElement element, bool parentIsCodeFilesArray = false)
        {
            var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            var isCodeFile = parentIsCodeFilesArray || IsCodeFileObject(element);

            foreach (var property in element.EnumerateObject())
            {
                var isCodeFileContent = isCodeFile
                    && property.Name.Equals("content", StringComparison.OrdinalIgnoreCase);
                var isSuggestedCode = property.Name.Equals("suggestedCode", StringComparison.OrdinalIgnoreCase);

                if (_fieldNames.Contains(property.Name) && !isCodeFileContent && !isSuggestedCode)
                {
                    result[property.Name] = SanitizeFieldValue(property.Value);
                }
                else if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    result[property.Name] = SanitizeObject(property.Value);
                }
                else if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    var isCodeFilesArray = property.Name.Equals("codeFiles", StringComparison.OrdinalIgnoreCase);
                    result[property.Name] = property.Value.EnumerateArray()
                        .Select(item => item.ValueKind == JsonValueKind.Object
                            ? SanitizeObject(item, isCodeFilesArray)
                            : SanitizeElement(item))
                        .ToList();
                }
                else
                {
                    result[property.Name] = SanitizeScalar(property.Value);
                }
            }

            return result;
        }

        private object? SanitizeFieldValue(JsonElement value) =>
            value.ValueKind switch
            {
                JsonValueKind.String => _sanitizer.Sanitize(value.GetString() ?? string.Empty),
                JsonValueKind.Array => value.EnumerateArray()
                    .Select(item => item.ValueKind == JsonValueKind.String
                        ? _sanitizer.Sanitize(item.GetString() ?? string.Empty)
                        : SanitizeElement(item))
                    .ToList(),
                JsonValueKind.Object => SanitizeObject(value),
                _ => SanitizeScalar(value)
            };

        private static object? SanitizeScalar(JsonElement element) =>
            element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => element.GetRawText()
            };

        private static HtmlSanitizer CreateSanitizer()
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedCssProperties.Clear();
            sanitizer.AllowedSchemes.Clear();
            sanitizer.AllowDataAttributes = false;
            return sanitizer;
        }
    }
}
