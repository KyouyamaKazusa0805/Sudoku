namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes which locked case a hidden subset is.
/// </summary>
/// <param name="size"><inheritdoc/></param>
/// <param name="isLocked">Indicates whether the hidden subset is locked.</param>
public sealed partial class HiddenSubsetIsLockedFactor(int size, [PrimaryConstructorParameter] bool isLocked) : SubsetFactor(size)
{
	/// <summary>
	/// Indicates the size value array.
	/// </summary>
	private static readonly int[] SizeValueArray = [0, 0, -12, -13];


	/// <inheritdoc/>
	public override Expression<Func<decimal>> Formula => () => (IsLocked ? SizeValueArray[Size] : 0) * Scale;


	/// <inheritdoc/>
	public override string GetName(CultureInfo? culture = null) => throw new NotImplementedException();
}
