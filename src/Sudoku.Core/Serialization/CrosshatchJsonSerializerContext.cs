namespace Sudoku.Serialization;

/// <summary>
/// Provides a serializer context on type <see cref="Presentation.Crosshatch"/>.
/// </summary>
/// <seealso cref="Presentation.Crosshatch"/>
[JsonSerializable(typeof(Crosshatch), GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class CrosshatchJsonSerializerContext : JsonSerializerContext
{
}
