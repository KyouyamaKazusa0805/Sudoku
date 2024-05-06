namespace Sudoku.Traits;

/// <summary>
/// Represents a trait that describes the length of complex chain in all branches.
/// </summary>
public interface IComplexChainLengthTrait : ITrait
{
	/// <summary>
	/// Indicates the length of complex chain.
	/// </summary>
	public abstract int ComplexLength { get; }
}
