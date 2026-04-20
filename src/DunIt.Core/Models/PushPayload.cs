namespace DunIt.Core.Models;

using System.Text.Json.Serialization;

public record PushPayload(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("body")] string Body);
