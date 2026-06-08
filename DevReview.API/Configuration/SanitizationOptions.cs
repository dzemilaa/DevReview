namespace DevReview.API.Configuration
{
    public class SanitizationOptions
    {
        public const string SectionName = "Sanitization";

        /// <summary>
        /// JSON property names (case-insensitive) sanitized on POST/PUT/PATCH bodies.
        /// </summary>
        public IList<string> Fields { get; set; } =
        [
            "title",
            "description",
            "content",
            "summary",
            "topic",
            "bookingDescription",
            "cancellationReason",
            "impression",
            "reason",
            "blockReason",
            "userName",
            "displayName",
            "email",
            "framework",
            "programmingLanguage",
            "tags",
            "codeFilePath",
            "message",
            "priorityFixes"
        ];
    }
}
