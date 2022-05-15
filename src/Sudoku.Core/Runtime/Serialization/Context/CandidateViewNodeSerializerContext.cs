namespace Sudoku.Runtime.Serialization.Context;

/// <summary>
/// Indicates the serializer context that binds with the type <see cref="Presentation.Nodes.CandidateViewNode"/>.
/// </summary>
/// <seealso cref="Presentation.Nodes.CandidateViewNode"/>
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified, GenerationMode = JsonSourceGenerationMode.Metadata, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(CandidateViewNode))]
internal sealed partial class CandidateViewNodeSerializerContext : JsonSerializerContext
{
}
