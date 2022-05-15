namespace Sudoku.Runtime.Serialization.Context;

/// <summary>
/// Indicates the serializer context that binds with the type <see cref="Presentation.Nodes.LinkViewNode"/>.
/// </summary>
/// <seealso cref="Presentation.Nodes.LinkViewNode"/>
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified, GenerationMode = JsonSourceGenerationMode.Metadata, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(LinkViewNode))]
internal sealed partial class LinkViewNodeSerializerContext : JsonSerializerContext
{
}
