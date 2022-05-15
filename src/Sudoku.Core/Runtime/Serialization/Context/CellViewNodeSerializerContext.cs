namespace Sudoku.Runtime.Serialization.Context;

/// <summary>
/// Indicates the serializer context that binds with the type <see cref="Presentation.Nodes.CellViewNode"/>.
/// </summary>
/// <seealso cref="Presentation.Nodes.CellViewNode"/>
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified, GenerationMode = JsonSourceGenerationMode.Metadata, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(CellViewNode))]
internal sealed partial class CellViewNodeSerializerContext : JsonSerializerContext
{
}
