namespace Sudoku.Inferring;

/// <summary>
/// Indicates the result value after <see cref="DeadlyPatternInferrer.TryInfer(ref readonly Grid, out DeadlyPatternInferredResult)"/> called.
/// </summary>
/// <param name="grid"><inheritdoc cref="Grid" path="/summary"/></param>
/// <param name="isDeadlyPattern">Indicates the pattern is a real deadly pattern.</param>
/// <param name="failedCases">
/// Indicates all possible failed cases. The value can be an empty sequence if the pattern is a real deadly pattern,
/// or not a deadly pattern but containing obvious invalid candidates (like containing given or modifiable cells).
/// </param>
/// <seealso cref="DeadlyPatternInferrer.TryInfer(ref readonly Grid, out DeadlyPatternInferredResult)"/>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlag.AllObjectMethods)]
public readonly ref partial struct DeadlyPatternInferredResult(
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", NamingRule = ">@")] ref readonly Grid grid,
	[PrimaryConstructorParameter] bool isDeadlyPattern,
	[PrimaryConstructorParameter] ReadOnlySpan<Grid> failedCases
)
{
	/// <summary>
	/// Indicates the candidates the pattern used.
	/// </summary>
	public CandidateMap PatternCandidates => Grid.Candidates.AsCandidateMap();
}
