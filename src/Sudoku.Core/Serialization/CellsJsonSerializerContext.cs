namespace Sudoku.Serialization;

/// <summary>
/// Provides a serializer context on type <see cref="Collections.Cells"/>.
/// </summary>
/// <seealso cref="Collections.Cells"/>
[JsonSerializable(typeof(Cells), GenerationMode = JsonSourceGenerationMode.Metadata)]
public sealed partial class CellsJsonSerializerContext : JsonSerializerContext
{
}
