namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes which locked case a naked subset is.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <param name="size"><inheritdoc/></param>
/// <param name="isLocked">Indicates whether the naked subset is locked.</param>
public sealed partial class NakedSubsetIsLockedFactor(
	StepSearcherOptions options,
	[PrimaryConstructorParameter] int size,
	[PrimaryConstructorParameter] bool? isLocked
) : Factor(options), ISizeTrait
{
	/// <summary>
	/// Indicates the size value array used when <see cref="IsLocked"/> is <see langword="true"/>.
	/// </summary>
	private static readonly int[] SizeValueArrayWhenTrue = [0, 0, -10, -11];

	/// <summary>
	/// Indicates the size value used when <see cref="IsLocked"/> is <see langword="false"/>.
	/// </summary>
	private static readonly int SizeValueWhenFalse = 1;


	/// <inheritdoc/>
	public override Expression<Func<decimal>> Formula
		=> () => IsLocked == true ? SizeValueArrayWhenTrue[Size] : IsLocked == false ? SizeValueWhenFalse : 0;
}
