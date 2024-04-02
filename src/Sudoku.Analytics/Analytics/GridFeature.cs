namespace Sudoku.Analytics;

/// <summary>
/// Represents extension methods on checking <see cref="Concepts.Grid"/> instances, determining features based on analytics.
/// </summary>
/// <param name="grid">Indicates the grid used.</param>
/// <seealso cref="Concepts.Grid"/>
[Equals]
[GetHashCode]
[ToString]
public readonly ref partial struct GridFeature(
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", GeneratedMemberName = "Grid")] ref readonly Grid grid
)
{
	/// <summary>
	/// Indicates whether the puzzle can be filled via only full house.
	/// </summary>
	public readonly bool CanOnlyUseFullHouse()
		=> Analyzers.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true })
			.WithConditionalOptions(new() { LimitedSingle = SingleTechnique.FullHouse })
			.Analyze(in Grid)
			.IsSolved;

	/// <summary>
	/// Indicates whether the puzzle can be filled, via only hidden singles and last digits.
	/// </summary>
	/// <param name="allowsLine">Indicates whether the method allows using crosshatching in rows or columns.</param>
	public readonly bool CanOnlyUseHiddenSingle(bool allowsLine)
		=> Analyzers.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true })
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true })
			.WithConditionalOptions(new() { LimitedSingle = SingleTechnique.HiddenSingle, AllowsHiddenSingleInLines = allowsLine })
			.Analyze(in Grid)
			.IsSolved;

	/// <summary>
	/// Indicates whether the puzzle can be filled, via only naked singles and full houses.
	/// </summary>
	public readonly bool CanOnlyUseNakedSingle()
		=> Analyzers.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true })
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true })
			.WithConditionalOptions(new() { LimitedSingle = SingleTechnique.NakedSingle })
			.Analyze(in Grid)
			.IsSolved;
}
