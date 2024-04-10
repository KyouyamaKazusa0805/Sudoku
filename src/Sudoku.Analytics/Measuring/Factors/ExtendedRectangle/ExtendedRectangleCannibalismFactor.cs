namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes cannibalism rule of <see cref="ExtendedRectangleType3Step"/>.
/// </summary>
/// <seealso cref="ExtendedRectangleType3Step"/>
public sealed class ExtendedRectangleCannibalismFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "{0} ? 2 : 0";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ExtendedRectangleType3Step.IsCannibalism)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExtendedRectangleType3Step);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			ExtendedRectangleType3Step { IsCannibalism: var isCannibalism } => isCannibalism ? 2 : 0,
			_ => null
		};
}
