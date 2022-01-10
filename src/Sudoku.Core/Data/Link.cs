namespace Sudoku.Data;

/// <summary>
/// Encapsulates a link used for drawing.
/// </summary>
/// <param name="StartCandidate">Indicates the start candidate.</param>
/// <param name="EndCandidate">Indicates the end candidate.</param>
/// <param name="LinkType">Indicates the link type.</param>
[AutoDeconstructLambda(nameof(StartCell), nameof(StartDigit), nameof(EndCell), nameof(EndDigit), nameof(LinkType))]
public readonly partial record struct Link(int StartCandidate, int EndCandidate, LinkType LinkType)
: IJsonSerializable<Link, Link.JsonConverter>
{
	/// <summary>
	/// Indicates the start cell.
	/// </summary>
	private int StartCell
	{
		[LambdaBody]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => StartCandidate / 9;
	}

	/// <summary>
	/// Indicates the start digit.
	/// </summary>
	private int StartDigit
	{
		[LambdaBody]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => StartCandidate % 9;
	}

	/// <summary>
	/// Indicates the end cell.
	/// </summary>
	private int EndCell
	{
		[LambdaBody]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => EndCandidate / 9;
	}

	/// <summary>
	/// Indicates the end digit.
	/// </summary>
	private int EndDigit
	{
		[LambdaBody]
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
		StartCandidate == other.StartCandidate
		&& EndCandidate == other.EndCandidate
		&& LinkType == other.LinkType;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => (int)LinkType << 20 | StartCandidate << 10 | EndCandidate;

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() =>
		$"{new Candidates { StartCandidate }}{LinkType.GetNotation()}{new Candidates { EndCandidate }}";
}
