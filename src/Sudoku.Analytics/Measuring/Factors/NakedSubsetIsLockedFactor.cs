namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes which locked case a naked subset is.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public sealed class NakedSubsetIsLockedFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		({0}, {1}) switch
		{
			(true, 2) => -10,
			(true, 3) => -11,
			(false, _) => 1,
			_ => 0
		}
		""";

	/// <inheritdoc/>
	public override PropertyInfo[] Parameters
		=> [
			typeof(NakedSubsetStep).GetProperty(nameof(NakedSubsetStep.IsLocked))!,
			typeof(NakedSubsetStep).GetProperty(nameof(NakedSubsetStep.Size))!
		];

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			NakedSubsetStep { IsLocked: var isLocked, Size: var size } => (isLocked, size) switch
			{
				(true, 2) => -10,
				(true, 3) => -11,
				(false, _) => 1,
				_ => 0
			},
			_ => null
		};
}
