namespace Sudoku.Data;

/// <summary>
/// Encapsulates a link used for drawing.
/// </summary>
/// <param name="StartCandidate">Indicates the start candidate.</param>
/// <param name="EndCandidate">Indicates the end candidate.</param>
/// <param name="LinkType">Indicates the link type.</param>
[AutoDeconstruct(nameof(StartCell), nameof(StartDigit), nameof(EndCell), nameof(EndDigit), nameof(LinkType))]
[AutoHashCode(nameof(EigenValue))]
[AutoEquality(nameof(StartCandidate), nameof(EndCandidate), nameof(LinkType))]
public readonly partial record struct Link(int StartCandidate, int EndCandidate, LinkType LinkType) : IValueEquatable<Link>, IJsonSerializable<Link, Link.JsonConverter>
{
	/// <summary>
	/// Indicates the start cell.
	/// </summary>
	private int StartCell => StartCandidate / 9;

	/// <summary>
	/// Indicates the start digit.
	/// </summary>
	private int StartDigit => StartCandidate % 9;

	/// <summary>
	/// Indicates the end cell.
	/// </summary>
	private int EndCell => EndCandidate / 9;

	/// <summary>
	/// Indicates the end digit.
	/// </summary>
	private int EndDigit => EndCandidate % 9;

	/// <summary>
	/// Indicates the eigen value.
	/// </summary>
	private int EigenValue => (int)LinkType << 20 | StartCandidate << 10 | EndCandidate;


	/// <inheritdoc cref="object.ToString"/>
	public override string ToString()
	{
		var sb = new ValueStringBuilder(stackalloc char[100]);
		sb.Append(new Candidates { StartCandidate }.ToString());
		sb.Append(LinkType.GetNotation());
		sb.Append(new Candidates { EndCandidate }.ToString());

		return sb.ToString();
	}
}
