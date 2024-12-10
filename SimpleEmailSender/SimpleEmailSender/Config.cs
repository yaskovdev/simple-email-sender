using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace SimpleEmailSender;

public record Config(
    [property: JsonPropertyName("sender")]
    string Sender,
    [property: JsonPropertyName("senderPassword")]
    string SenderPassword,
    [property: JsonPropertyName("testRecipients")]
    IImmutableList<string> TestRecipients,
    [property: JsonPropertyName("recipients")]
    IImmutableList<string> Recipients
);