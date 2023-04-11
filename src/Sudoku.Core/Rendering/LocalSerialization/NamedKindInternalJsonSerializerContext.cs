namespace Sudoku.Rendering.LocalSerialization;

/// <summary>
/// Provides with a serializer context that uses source generator to produce necessary code on serialization
/// or deserialization on type <see cref="NamedKindInternal"/>.
/// </summary>
/// <seealso cref="NamedKindInternal"/>
[JsonSerializable(typeof(NamedKindInternal))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true)]
internal sealed partial class NamedKindInternalJsonSerializerContext : JsonSerializerContext;
