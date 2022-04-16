namespace Sudoku.Runtime.Serialization.Context;

/// <summary>
/// Indicates the serializer context that binds
/// with the type <see cref="Presentation.Nodes.CrosshatchViewNode"/>.
/// </summary>
/// <seealso cref="Presentation.Nodes.CrosshatchViewNode"/>
[JsonSourceGenerationOptions(
	WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
	GenerationMode = JsonSourceGenerationMode.Metadata,
	DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(CrosshatchViewNode))]
internal sealed partial class CrosshatchViewNodeSerializerContext : JsonSerializerContext
{
}
