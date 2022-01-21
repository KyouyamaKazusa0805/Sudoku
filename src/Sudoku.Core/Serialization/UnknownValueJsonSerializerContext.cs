namespace Sudoku.Serialization;

/// <summary>
/// Provides a serializer context on type <see cref="Presentation.UnknownValue"/>.
/// </summary>
/// <seealso cref="Presentation.UnknownValue"/>
[JsonSerializable(typeof(UnknownValue), GenerationMode = JsonSourceGenerationMode.Metadata)]
internal sealed partial class UnknownValueJsonSerializerContext : JsonSerializerContext
{
}
