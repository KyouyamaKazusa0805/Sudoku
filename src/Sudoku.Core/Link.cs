using Sudoku.Collections;

namespace Sudoku;

/// <summary>
/// Encapsulates a link used for drawing.
/// </summary>
/// <param name="Mask">
/// The mask of the link. The mask contains 21 bits used:
/// <code>
/// 24      16      8       0
/// |       |       |       |
/// |-------|-------|-------|
/// |   ||        |         |
/// 24 2120       10        0
/// </code>
/// </param>
public readonly record struct Link(int Mask) :
	IDefaultable<Link>,
	IEquatable<Link>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<Link, Link>
#endif
{
	/// <inheritdoc cref="IDefaultable{T}.Default"/>
	public static readonly Link Default;


	/// <summary>
	/// Initializes a <see cref="Link"/> instance via the start and end candidate, and the kind of the link.
	/// </summary>
	/// <param name="startCandidate">The start candidate.</param>
	/// <param name="endCandidate">The end candidate.</param>
	/// <param name="kind">The kind of the link.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Link(int startCandidate, int endCandidate, LinkKind kind) :
		this((int)kind << 20 | startCandidate << 10 | endCandidate)
	{
	}


	/// <summary>
	/// Indicates the start candidate.
	/// </summary>
	public int StartCandidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Mask >> 10 & 1023;
	}

	/// <summary>
	/// Indicates the start cell.
	/// </summary>
	public int StartCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Mask >> 10 & 1023) / 9;
	}

	/// <summary>
	/// Indicates the start digit.
	/// </summary>
	public int StartDigit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Mask >> 10 & 1023) % 9;
	}

	/// <summary>
	/// Indicates the end candidate.
	/// </summary>
	public int EndCandidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Mask & 1023;
	}

	/// <summary>
	/// Indicates the end cell.
	/// </summary>
	public int EndCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Mask & 1023) / 9;
	}

	/// <summary>
	/// Indicates the end digit.
	/// </summary>
	public int EndDigit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Mask & 1023) % 9;
	}

	/// <summary>
	/// Indicates the link kind.
	/// </summary>
	public LinkKind Kind
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (LinkKind)(Mask >> 20 & 3);
	}

	/// <inheritdoc/>
	bool IDefaultable<Link>.IsDefault => this == Default;

	/// <inheritdoc/>
	static Link IDefaultable<Link>.Default => Default;


	/// <summary>
	/// Determine whether the specified <see cref="Link"/> instance holds the same start cell,
	/// end cell and the link type as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Link other) => Mask == other.Mask;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => Mask;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{new Candidates { StartCandidate }}{Kind.GetNotation()}{new Candidates { EndCandidate }}";
}
