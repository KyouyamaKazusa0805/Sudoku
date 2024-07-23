namespace Sudoku.Inferring;

/// <summary>
/// Indicates the result value after <see cref="DeadlyPatternInferrer.TryInfer(ref readonly Grid, out DeadlyPatternInferredResult)"/> called.
/// </summary>
/// <param name="grid">Indicates the pattern to be checked.</param>
/// <param name="isDeadlyPattern">Indicates the pattern is a real deadly pattern.</param>
/// <seealso cref="DeadlyPatternInferrer.TryInfer(ref readonly Grid, out DeadlyPatternInferredResult)"/>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlag.AllObjectMethods)]
public readonly ref partial struct DeadlyPatternInferredResult(ref readonly Grid grid, [PrimaryConstructorParameter] bool isDeadlyPattern)
{
	/// <summary>
	/// <inheritdoc cref="DeadlyPatternInferredResult(ref readonly Grid, bool)" path="/param[@name='grid']"/>
	/// </summary>
	public Grid Grid { get; } = grid;

	/// <summary>
	/// Indicates the candidates the pattern used.
	/// </summary>
	public CandidateMap PatternCandidates => Grid.Candidates.AsCandidateMap();
}
