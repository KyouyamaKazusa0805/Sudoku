namespace Sudoku.Inferring;

/// <summary>
/// Indicates the result value after <see cref="DeadlyPatternInferrer.TryInfer(in Grid, out DeadlyPatternInferredResult)"/> called.
/// </summary>
/// <param name="grid"><inheritdoc cref="Grid" path="/summary"/></param>
/// <param name="isDeadlyPattern">Indicates the pattern is a real deadly pattern.</param>
/// <param name="failedCases">
/// Indicates all possible failed cases. The value can be an empty sequence if the pattern is a real deadly pattern,
/// or not a deadly pattern but containing obvious invalid candidates (like containing given or modifiable cells).
/// </param>
/// <param name="patternCandidates">Indicates the candidates the pattern used.</param>
/// <seealso cref="DeadlyPatternInferrer.TryInfer(in Grid, out DeadlyPatternInferredResult)"/>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public readonly ref partial struct DeadlyPatternInferredResult(
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] in Grid grid,
	[Property] bool isDeadlyPattern,
	[Property] ReadOnlySpan<Grid> failedCases,
	[Property(RefKind = null)] scoped in CandidateMap patternCandidates
);
