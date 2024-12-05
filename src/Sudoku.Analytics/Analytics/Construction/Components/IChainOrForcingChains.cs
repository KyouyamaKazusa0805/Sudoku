namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a component that is a chain or forcing chains.
/// </summary>
public interface IChainOrForcingChains : IComponent
{
	/// <summary>
	/// Indicates whether the pattern is grouped (i.e. contains a node uses at least 2 candidates).
	/// </summary>
	public abstract bool IsGrouped { get; }

	/// <summary>
	/// Indicates whether the pattern is strictly grouped,
	/// meaning at least one link (no matter what kind of link, strong or weak) uses advanced checking rules like AUR and ALS,
	/// or returns <see langword="true"/> from property <see cref="IsGrouped"/>.
	/// </summary>
	/// <seealso cref="IsGrouped"/>
	public abstract bool IsStrictlyGrouped { get; }

	/// <inheritdoc/>
	DataStructureType IDataStructure.Type => DataStructureType.LinkedList;

	/// <inheritdoc/>
	DataStructureBase IDataStructure.Base => DataStructureBase.LinkedListBased;


	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public abstract string ToString(IFormatProvider? formatProvider);
}
