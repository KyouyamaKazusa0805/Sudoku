namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Represents a role that describes for a conjugate pair.
/// </summary>
/// <typeparam name="TSelf">The type of the implementation.</typeparam>
/// <typeparam name="THouseMask">The type of the house mask.</typeparam>
/// <typeparam name="TMask">The type of the bit mask.</typeparam>
/// <typeparam name="TCell">The type of the cell.</typeparam>
/// <typeparam name="TDigit">The type of the digit.</typeparam>
/// <typeparam name="THouse">The type of the house.</typeparam>
/// <typeparam name="TBitStatusMap">The type of the bit status map.</typeparam>
public interface IConjugatePair<TSelf, THouseMask, TMask, TCell, TDigit, THouse, TBitStatusMap> :
	IEquatable<TSelf>,
	IEqualityOperators<TSelf, TSelf, bool>
	where TSelf : IConjugatePair<TSelf, THouseMask, TMask, TCell, TDigit, THouse, TBitStatusMap>
	where THouseMask : unmanaged, IBinaryInteger<THouseMask>
	where TMask : unmanaged, IBinaryInteger<TMask>
	where TCell : unmanaged, IBinaryInteger<TCell>
	where TDigit : unmanaged, IBinaryInteger<TDigit>
	where THouse : unmanaged, IBinaryInteger<THouse>
	where TBitStatusMap : unmanaged, IBitStatusMap<TBitStatusMap, TCell>
{
	/// <summary>
	/// Indicates the cell that starts with the conjugate pair.
	/// </summary>
	public abstract TCell From { get; }

	/// <summary>
	/// Indicates the cell that ends with the conjugate pair.
	/// </summary>
	public abstract TCell To { get; }

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public abstract TDigit Digit { get; }

	/// <summary>
	/// Indicates the line that two cells lie in.
	/// </summary>
	public abstract THouse Line { get; }

	/// <summary>
	/// Indicates the full map of cells used.
	/// </summary>
	public virtual TBitStatusMap Map => TBitStatusMap.Empty + From + To;

	/// <summary>
	/// Indicates the house that two cells lie in.
	/// </summary>
	/// <remarks><inheritdoc cref="CellMap.CoveredHouses"/></remarks>
	public abstract THouseMask Houses { get; }

	/// <summary>
	/// Indicates the backing mask value.
	/// </summary>
	public abstract TMask Mask { get; }
}
