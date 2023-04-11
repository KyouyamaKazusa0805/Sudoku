namespace Sudoku.Rendering.LocalSerialization;

/// <summary>
/// Provides with a serializer context that uses source generator to produce necessary code on serialization
/// or deserialization on type <see cref="LocalSerialization.ColorInternal"/>.
/// </summary>
/// <seealso cref="LocalSerialization.ColorInternal"/>
[JsonSerializable(typeof(ColorInternal))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true)]
internal sealed partial class ColorInternalSerializerContext : JsonSerializerContext;
