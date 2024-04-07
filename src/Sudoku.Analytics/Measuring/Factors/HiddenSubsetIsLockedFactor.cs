namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes which locked case a hidden subset is.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <param name="size"><inheritdoc/></param>
/// <param name="isLocked">Indicates whether the hidden subset is locked.</param>
public sealed partial class HiddenSubsetIsLockedFactor(
	StepSearcherOptions options,
	[PrimaryConstructorParameter] int size,
	[PrimaryConstructorParameter] bool isLocked
) : Factor(options), ISizeTrait
{
	/// <summary>
	/// Indicates the size value array.
	/// </summary>
	private static readonly int[] SizeValueArray = [0, 0, -12, -13];


	/// <inheritdoc/>
	public override Expression<Func<decimal>> Formula => () => IsLocked ? SizeValueArray[Size] : 0;
}
