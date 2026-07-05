using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace ITSupportNative.Desktop.Models;

public sealed record ShellRoute(string Key, string Label, string Glyph);

public sealed record QuickAction(
    string Title,
    string Description,
    string Glyph,
    string RouteKey);

public sealed record StatusMetric(
    string Label,
    string Value,
    string Detail,
    string Glyph,
    string Tone);

public sealed record SoftwarePreview(
    string Name,
    string Category,
    string Version,
    string Status,
    string Glyph);

public sealed record ActivityPreview(
    string Title,
    string Detail,
    string Timestamp,
    string Glyph);

public sealed record AssistantSuggestion(
    string Title,
    string Prompt,
    string Glyph,
    ICommand Command);

public sealed record AssistantChatMessage(
    string Author,
    string Body,
    string Detail,
    string Glyph,
    HorizontalAlignment Alignment);

public sealed record RequestPreview(
    string Reference,
    string Title,
    string State,
    string UpdatedAt,
    string Detail);
