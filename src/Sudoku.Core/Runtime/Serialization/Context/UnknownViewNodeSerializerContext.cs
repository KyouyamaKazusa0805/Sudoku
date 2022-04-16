namespace Sudoku.Runtime.Serialization.Context;

/// <summary>
/// Indicates the serializer context that binds with the type <see cref="Presentation.Nodes.UnknownViewNode"/>.
/// </summary>
/// <seealso cref="Presentation.Nodes.UnknownViewNode"/>
[JsonSourceGenerationOptions(
	WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
	GenerationMode = JsonSourceGenerationMode.Metadata,
	DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(UnknownViewNode))]
internal sealed partial class UnknownViewNodeSerializerContext : JsonSerializerContext
{
}
