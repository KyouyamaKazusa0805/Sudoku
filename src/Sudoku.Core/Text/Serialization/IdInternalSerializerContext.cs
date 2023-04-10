namespace Sudoku.Text.Serialization;

/// <summary>
/// Provides with a serializer context that uses source generator to produce necessary code on serialization
/// or deserialization on type <see cref="IdInternal"/>.
/// </summary>
/// <seealso cref="IdInternal"/>
[JsonSerializable(typeof(IdInternal))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true)]
internal sealed partial class IdInternalSerializerContext : JsonSerializerContext;
