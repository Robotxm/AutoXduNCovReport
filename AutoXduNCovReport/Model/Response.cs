using System.Text.Json.Serialization;

namespace AutoXduNCovReport.Model
{
    /// <summary>
    /// Represents the response of report system.
    /// </summary>
    /// <typeparam name="T">The type of Data.</typeparam>
    record BaseResponse<T>
    (
        [property: JsonPropertyName("e")] int Code,
        [property: JsonPropertyName("d")] T Data,
        [property: JsonPropertyName("m")] string Message
    );

    record TCheckData(
        [property: JsonPropertyName("ontime")] bool OnTime,
        [property: JsonPropertyName("realonly")] bool Readonly
    );
}