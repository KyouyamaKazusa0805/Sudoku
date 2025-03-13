namespace Sudoku.Behaviors.Bottlenecks;

/// <summary>
/// Represents a bottleneck filter consumed by method <see cref="AnalysisResultExtensions.GetBottlenecks(AnalysisResult, ReadOnlySpan{BottleneckFilter})"/>.
/// </summary>
/// <param name="Visibility">Indicates a visibility type of pencilmark.</param>
/// <param name="Type">Indicates the bottleneck type.</param>
/// <seealso cref="AnalysisResultExtensions.GetBottlenecks(AnalysisResult, ReadOnlySpan{BottleneckFilter})"/>
public record struct BottleneckFilter(PencilmarkVisibility Visibility, BottleneckType Type);
