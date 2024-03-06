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
	/// Indicates whether the current grid is an ittoryu puzzle, solved in only one round.
	/// </summary>
	public bool IsIttoryu()
	{
		var analyzer = PredefinedAnalyzers.Default
			.WithStepSearchers([new SingleStepSearcher { EnableFullHouse = true, HiddenSinglesInBlockFirst = true, UseIttoryuMode = true }])
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true });
		if (analyzer.Analyze(in Grid) is not { IsSolved: true, Steps: var steps })
		{
			return false;
		}

		for (var i = 0; i < steps.Length - 1; i++)
		{
			var a = ((SingleStep)steps[i]).Digit;
			var b = ((SingleStep)steps[i + 1]).Digit;
			if ((a, b) is (8, 0))
			{
				return false;
			}

			if (b - a is 0 or 1)
			{
				continue;
			}

			return false;
		}

		return true;
	}

	/// <summary>
	/// Indicates whether the puzzle can be filled, via only hidden singles and last digits.
	/// </summary>
	/// <param name="allowsLine">Indicates whether the method allows using crosshatching in rows or columns.</param>
	public readonly bool CanOnlyUseHiddenSingle(bool allowsLine)
		=> PredefinedAnalyzers.Default
			.WithStepSearchers([new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true }])
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true, })
			.WithConditionalOptions(new() { LimitedSingle = SingleTechnique.HiddenSingle, AllowsHiddenSingleInLines = allowsLine })
			.Analyze(in Grid)
			.IsSolved;

	/// <summary>
	/// Indicates whether the puzzle can be filled, via only naked singles and full houses.
	/// </summary>
	public readonly bool CanOnlyUseNakedSingle()
		=> PredefinedAnalyzers.Default
			.WithStepSearchers([new SingleStepSearcher { EnableFullHouse = true }])
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true })
			.WithConditionalOptions(new() { LimitedSingle = SingleTechnique.NakedSingle })
			.Analyze(in Grid)
			.IsSolved;
}
