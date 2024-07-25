namespace Sudoku.Concepts.Primitives;

/// <summary>
/// Represents constant values for type.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface IGridConstants<TSelf> : IEqualityOperators<TSelf, TSelf, bool> where TSelf : unmanaged, IGridConstants<TSelf>
{
	/// <summary>
	/// Indicates the number of cells of a sudoku grid.
	/// </summary>
	public const Cell CellsCount = 81;

	/// <summary>
	/// Indicates the number of digits can be appeared inside a cell.
	/// </summary>
	public const Digit CellCandidatesCount = 9;


	/// <summary>
	/// Indicates whether the grid is <see cref="Empty"/>, which means the grid holds totally same value with <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	public virtual bool IsEmpty => (TSelf)this == TSelf.Empty;

	/// <summary>
	/// Indicates whether the grid is <see cref="Undefined"/>, which means the grid holds totally same value with <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public virtual bool IsUndefined => (TSelf)this == TSelf.Undefined;


	/// <summary>
	/// Represents a string value that describes a <typeparamref name="TSelf"/> instance can be parsed into <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	public static abstract string EmptyString { get; }

	/// <summary>
	/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
	/// </summary>
	public static virtual Mask DefaultMask => (Mask)(TSelf.EmptyMask | TSelf.MaxCandidatesMask);

	/// <summary>
	/// Indicates the empty mask, modifiable mask and given mask.
	/// </summary>
	public static abstract Mask EmptyMask { get; }

	/// <summary>
	/// Indicates the modifiable mask.
	/// </summary>
	public static abstract Mask ModifiableMask { get; }

	/// <summary>
	/// Indicates the given mask.
	/// </summary>
	public static abstract Mask GivenMask { get; }

	/// <summary>
	/// Indicates the maximum candidate mask that used.
	/// </summary>
	public static abstract Mask MaxCandidatesMask { get; }

	/// <summary>
	/// The empty grid that is valid during implementation or running the program
	/// (all values are <see cref="DefaultMask"/>, i.e. empty cells).
	/// </summary>
	/// <remarks>
	/// This field is initialized by the static constructor of this structure.
	/// </remarks>
	/// <seealso cref="DefaultMask"/>
	public static abstract ref readonly TSelf Empty { get; }

	/// <summary>
	/// Indicates the default grid that all values are initialized 0.
	/// This value is equivalent to <see langword="default"/>(<typeparamref name="TSelf"/>).
	/// </summary>
	/// <remarks>
	/// This value can be used for non-candidate-based sudoku operations, e.g. a sudoku grid canvas.
	/// </remarks>
	public static abstract ref readonly TSelf Undefined { get; }
}
