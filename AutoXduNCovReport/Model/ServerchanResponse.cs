using System.Text.Json.Serialization;

namespace AutoXduNCovReport.Model
{
    /// <summary>
    /// Represents the response of Serverchan.
    /// </summary>
    record ServerchanResponse (
        [property: JsonPropertyName("errno")] int Code,
        [property: JsonPropertyName("dataset")]
        string? Data,
        [property: JsonPropertyName("errmsg")] string Message
    );
}