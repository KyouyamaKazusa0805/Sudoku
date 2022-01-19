namespace Sudoku.Serialization;

/// <summary>
/// Provides a serializer context on type <see cref="Data.Cells"/>.
/// </summary>
/// <seealso cref="Data.Cells"/>
[JsonSerializable(typeof(Cells), GenerationMode = JsonSourceGenerationMode.Metadata)]
public sealed partial class CellsJsonSerializerContext : JsonSerializerContext
{
}
