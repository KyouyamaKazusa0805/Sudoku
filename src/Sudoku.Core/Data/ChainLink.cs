namespace Sudoku.Data;

/// <summary>
/// Encapsulates a link used for drawing.
/// </summary>
/// <param name="StartCandidate">Indicates the start candidate.</param>
/// <param name="EndCandidate">Indicates the end candidate.</param>
/// <param name="LinkType">Indicates the link type.</param>
[AutoDeconstructLambda(nameof(StartCell), nameof(StartDigit), nameof(EndCell), nameof(EndDigit), nameof(LinkType))]
[AutoGetHashCode(nameof(EigenValue))]
[AutoEquality(nameof(StartCandidate), nameof(EndCandidate), nameof(LinkType))]
public readonly partial record struct ChainLink(int StartCandidate, int EndCandidate, ChainLinkType LinkType)
: IValueEquatable<ChainLink>
, IJsonSerializable<ChainLink, ChainLink.JsonConverter>
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
	/// Indicates the eigen value.
	/// </summary>
	private int EigenValue
	{
		[LambdaBody]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (int)LinkType << 20 | StartCandidate << 10 | EndCandidate;
	}


	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() =>
		$"{new Candidates { StartCandidate }}{LinkType.GetNotation()}{new Candidates { EndCandidate }}";
}
