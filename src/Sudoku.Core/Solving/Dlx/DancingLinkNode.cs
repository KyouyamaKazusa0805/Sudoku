namespace Sudoku.Solving.Dlx;

/// <summary>
/// Represents a dancing link node.
/// </summary>
[DebuggerDisplay($$"""{{{nameof(ToString)}}(),nq}""")]
[TypeImpl(TypeImplFlag.Object_ToString)]
public partial class DancingLinkNode : IFormattable
{
	/// <summary>
	/// Initializes a <see cref="DancingLinkNode"/> instance via the specified ID value and the column node.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DancingLinkNode(Candidate candidate)
		=> (Candidate, Column, Left, Right, Up, Down) = (candidate, null, this, this, this, this);

	/// <summary>
	/// Initializes a <see cref="DancingLinkNode"/> instance via the specified ID value and the column node.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <param name="column">The column node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DancingLinkNode(Candidate candidate, ColumnNode? column)
		=> (Candidate, Column, Left, Right, Up, Down) = (candidate, column, this, this, this, this);


	/// <summary>
	/// Indicates the candidate of the node.
	/// </summary>
	public Candidate Candidate { get; set; }

	/// <summary>
	/// Indicates the current column node.
	/// </summary>
	public ColumnNode? Column { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the left node.
	/// </summary>
	public DancingLinkNode Left { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the right node.
	/// </summary>
	public DancingLinkNode Right { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the up node.
	/// </summary>
	public DancingLinkNode Up { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the down node.
	/// </summary>
	public DancingLinkNode Down { get; set; }


	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public string ToString(IFormatProvider? formatProvider)
	{
		var sb = new StringBuilder();
		sb.Append($"{nameof(DancingLinkNode)} {{ ");
		sb.Append($"{nameof(Candidate)} = {f(Candidate)}, ");
		sb.Append($"{nameof(Up)} = {f(Up.Candidate)}, ");
		sb.Append($"{nameof(Down)} = {f(Down.Candidate)}, ");
		sb.Append($"{nameof(Left)} = {f(Left.Candidate)}, ");
		sb.Append($"{nameof(Right)} = {f(Right.Candidate)} }}");
		return sb.ToString();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		string f(Candidate candidate)
		{
			var converter = CoordinateConverter.GetInstance(formatProvider);
			return candidate == -1
				? "<invalid>"
				: candidate switch
				{
					< 81 when candidate.AsCellMap() is var map => map.ToString(converter),
					< 162 when ((candidate - 81) / 9, candidate % 9) is var (row, digit)
						=> $"{converter.HouseConverter(1 << row + 9)}({digit + 1})",
					< 243 when ((candidate - 162) / 9, candidate % 9) is var (column, digit)
						=> $"{converter.HouseConverter(1 << column + 18)}({digit + 1})",
					_ when ((candidate - 243) / 9, candidate % 9) is var (block, digit)
						=> $"{converter.HouseConverter(1 << block)}({digit + 1})"
				};
		}
	}

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
