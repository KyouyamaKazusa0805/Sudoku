namespace Sudoku.Runtime.Serialization.Context;

/// <summary>
/// Indicates the serializer context that binds with the type <see cref="Presentation.Nodes.HouseViewNode"/>.
/// </summary>
/// <seealso cref="Presentation.Nodes.HouseViewNode"/>
[JsonSourceGenerationOptions(
	WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
	GenerationMode = JsonSourceGenerationMode.Metadata,
	DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(HouseViewNode))]
internal sealed partial class HouseViewNodeSerializerContext : JsonSerializerContext
{
}
