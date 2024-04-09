namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures value cell existence for <see cref="AlmostLockedCandidatesStep"/>.
/// </summary>
/// <seealso cref="AlmostLockedCandidatesStep"/>
public sealed class AlmostLockedCandidatesValueCellExistenceFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		({0}, {1}) switch
		{{
			(true, 2 or 3) => 1,
			(true, 4) => 2,
			_ => 0
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames
		=> [nameof(AlmostLockedCandidatesStep.HasValueCell), nameof(AlmostLockedCandidatesStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(AlmostLockedCandidatesStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			AlmostLockedCandidatesStep { HasValueCell: var hasValueCell, Size: var size } => hasValueCell switch
			{
				true => size switch { 2 or 3 => 1, 4 => 2 },
				_ => 0
			},
			_ => null
		};
}
