namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents the size factor used in hidden subsets.
/// </summary>
/// <param name="size"><inheritdoc/></param>
public sealed partial class HiddenSubsetSizeFactor([PrimaryConstructorParameter] int size) : SubsetFactor(size)
{
	/// <summary>
	/// Indicates the size value array to be used.
	/// </summary>
	private static readonly int[] SizeValueArray = [0, 0, 0, 6, 20];


	/// <inheritdoc/>
	public override Expression<Func<decimal>> Formula => () => SizeValueArray[Size] * Scale;


	/// <inheritdoc/>
	public override string GetName(CultureInfo? culture = null) => throw new NotImplementedException();
}
