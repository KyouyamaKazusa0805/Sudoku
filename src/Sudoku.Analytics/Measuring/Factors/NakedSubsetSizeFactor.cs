namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents the size factor used in naked subsets.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <param name="size"><inheritdoc/></param>
public sealed partial class NakedSubsetSizeFactor(StepSearcherOptions options, [PrimaryConstructorParameter] int size) :
	Factor(options),
	ISizeTrait
{
	/// <summary>
	/// Indicates the size value array to be used.
	/// </summary>
	private static readonly int[] SizeValueArray = [0, 0, 0, 6, 20];


	/// <inheritdoc/>
	public override Expression<Func<decimal>> Formula => () => SizeValueArray[Size];
}
