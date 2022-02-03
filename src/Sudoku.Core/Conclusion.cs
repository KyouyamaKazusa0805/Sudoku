using System.ComponentModel;
using Sudoku.Collections;
using Sudoku.Presentation;

namespace Sudoku;

/// <summary>
/// Encapsulates a conclusion representation while solving in logic.
/// </summary>
/// <param name="Mask">
/// Indicates the mask that holds the information for the cell, digit and the conclusion type.
/// The bits distribution is like:
/// <code><![CDATA[
/// 16       8       0
///  |-------|-------|
///  |     |---------|
/// 16    10         0
///        |   used  |
/// ]]></code>
/// </param>
/// <remarks>
/// Two <see cref="Conclusion"/>s can be compared with each other. If one of those two is an elimination
/// (i.e. holds the value <see cref="ConclusionType.Elimination"/> as the type), the instance
/// will be greater; if those two hold same conclusion type, but one of those two holds
/// the global index of the candidate position is greater, it is greater.
/// </remarks>
/// <seealso cref="ConclusionType.Elimination"/>
public readonly record struct Conclusion(int Mask) :
	IComparable<Conclusion>,
	IDefaultable<Conclusion>,
	IEquatable<Conclusion>
#if FEATURE_GENERIC_MATH
	,
	IComparisonOperators<Conclusion, Conclusion>,
	IEqualityOperators<Conclusion, Conclusion>
#endif
{
	/// <summary>
	/// <inheritdoc cref="IDefaultable{T}.Default"/>
	/// </summary>
	public static readonly Conclusion Default = default;


	/// <summary>
	/// Initializes an instance with a conclusion type and a candidate offset.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="candidate">The candidate offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int candidate) : this(((int)type << 1) + candidate)
	{
	}

	/// <summary>
	/// Initializes the <see cref="Conclusion"/> instance via the specified cell, digit and the conclusion type.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int cell, int digit) : this(((int)type << 1) + cell * 9 + digit)
	{
	}


	/// <summary>
	/// Indicates the cell.
	/// </summary>
	public int Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <summary>
	/// Indicates the candidate.
	/// </summary>
	public int Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Mask & ((1 << 10) - 1);
	}

	/// <summary>
	/// The conclusion type to control the action of applying.
	/// If the type is <see cref="ConclusionType.Assignment"/>,
	/// this conclusion will be set value (Set a digit into a cell);
	/// otherwise, a candidate will be removed.
	/// </summary>
	public ConclusionType ConclusionType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ConclusionType)(Mask >> 10 & 1);
	}

	/// <inheritdoc/>
	bool IDefaultable<Conclusion>.IsDefault
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == default;
	}

	/// <inheritdoc/>
	static Conclusion IDefaultable<Conclusion>.Default
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Default;
	}


	/// <summary>
	/// Deconstruct the instance into multiple values.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ConclusionType conclusionType, out int candidate)
	{
		conclusionType = (ConclusionType)(Mask >> 10 & 1);
		candidate = Mask & ((1 << 10) - 1);
	}

	/// <summary>
	/// Deconstruct the instance into multiple values.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ConclusionType conclusionType, out int cell, out int digit)
	{
		conclusionType = (ConclusionType)(Mask >> 10 & 1);
		cell = Candidate / 9;
		digit = Candidate % 9;
	}

	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTo(ref Grid grid)
	{
		switch (ConclusionType)
		{
			case ConclusionType.Assignment:
			{
				grid[Cell] = Digit;
				break;
			}
			case ConclusionType.Elimination:
			{
				grid[Cell, Digit] = false;
				break;
			}
		}
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conclusion other) => Mask == other.Mask;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => Mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Conclusion other) => Mask - other.Mask;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{new Coordinate((byte)Cell)}{ConclusionType.GetNotation()}{Digit + 1}";

#if FEATURE_GENERIC_MATH
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable.CompareTo(object? obj) =>
		obj is Conclusion comparer
			? CompareTo(comparer)
			: throw new ArgumentException($"The argument must be of type '{nameof(Conclusion)}'", nameof(obj));
#endif


	/// <inheritdoc/>
	public static bool operator <(Conclusion left, Conclusion right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	public static bool operator <=(Conclusion left, Conclusion right) => left.CompareTo(right) <= 0;

	/// <inheritdoc/>
	public static bool operator >(Conclusion left, Conclusion right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	public static bool operator >=(Conclusion left, Conclusion right) => left.CompareTo(right) >= 0;
}
