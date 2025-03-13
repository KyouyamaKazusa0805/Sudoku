namespace Sudoku.Analytics;

/// <summary>
/// Represents a bottleneck filter consumed by method <see cref="AnalysisResult.GetBottlenecks(ReadOnlySpan{BottleneckFilter})"/>.
/// </summary>
/// <param name="Visibility">Indicates a visibility type of pencilmark.</param>
/// <param name="Type">Indicates the bottleneck type.</param>
/// <seealso cref="AnalysisResult.GetBottlenecks(ReadOnlySpan{BottleneckFilter})"/>
public record struct BottleneckFilter(PencilmarkVisibility Visibility, BottleneckType Type);
