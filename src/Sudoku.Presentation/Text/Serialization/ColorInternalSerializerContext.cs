namespace Sudoku.Text.Serialization;

/// <summary>
/// Provides with a serializer context that uses source generator to produce necessary code on serialization
/// or deserialization on type <see cref="ColorInternal"/>.
/// </summary>
/// <seealso cref="ColorInternal"/>
[JsonSerializable(typeof(ColorInternal))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true)]
internal sealed partial class ColorInternalSerializerContext : JsonSerializerContext;
