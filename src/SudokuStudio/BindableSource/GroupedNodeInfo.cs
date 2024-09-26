namespace SudokuStudio.BindableSource;

/// <summary>
/// Indicates the information that describes a grouped node.
/// </summary>
/// <param name="Map">Indicates the map.</param>
public sealed record GroupedNodeInfo(ref readonly CandidateMap Map) : IDrawableItem;
