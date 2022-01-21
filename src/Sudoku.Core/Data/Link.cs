namespace Sudoku.Data;

/// <summary>
/// Encapsulates a link used for drawing.
/// </summary>
/// <param name="StartCandidate">Indicates the start candidate.</param>
/// <param name="EndCandidate">Indicates the end candidate.</param>
/// <param name="LinkType">Indicates the link type.</param>
public readonly record struct Link(int StartCandidate, int EndCandidate, LinkType LinkType)
{
	/// <summary>
	/// Indicates the start cell.
	/// </summary>
	public int StartCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => StartCandidate / 9;
	}

	/// <summary>
	/// Indicates the start digit.
	/// </summary>
	public int StartDigit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => StartCandidate % 9;
	}

	/// <summary>
	/// Indicates the end cell.
	/// </summary>
	public int EndCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => EndCandidate / 9;
	}

	/// <summary>
	/// Indicates the end digit.
	/// </summary>
	public int EndDigit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => EndCandidate % 9;
	}


	/// <summary>
	/// Determine whether the specified <see cref="Link"/> instance holds the same start cell,
	/// end cell and the link type as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Link other) =>
		StartCandidate == other.StartCandidate && EndCandidate == other.EndCandidate && LinkType == other.LinkType;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => (int)LinkType << 20 | StartCandidate << 10 | EndCandidate;

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() =>
		$"{new Candidates { StartCandidate }}{LinkType.GetNotation()}{new Candidates { EndCandidate }}";
}
