#if NATIVE_AOT || DEBUG
using System.Text.Json.Serialization;

namespace Sudoku.Concepts.Serialization;

/// <summary>
/// Represents a <see cref="JsonSerializerContext"/> instance that can serialize and deserialize for a <see cref="string"/>[] values.
/// SUch raw values can be used for <see cref="CellMap"/> and <see cref="CandidateMap"/>.
/// </summary>
/// <seealso cref="CellMap"/>
/// <seealso cref="CandidateMap"/>
[JsonSerializable(typeof(string[]), GenerationMode = JsonSourceGenerationMode.Serialization, TypeInfoPropertyName = "Target")]
internal sealed partial class CellMapAndCandidateMapRawValueSerializationContext : JsonSerializerContext;
#endif