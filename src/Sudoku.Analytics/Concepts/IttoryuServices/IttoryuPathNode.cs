namespace Sudoku.Runtime.IttoryuServices;

/// <summary>
/// Represents for a path node in a whole solving path via ittoryu solving logic.
/// </summary>
/// <param name="Grid">Indicates the currently-used grid.</param>
/// <param name="House">Indicates the house. The value can be -1 when the represented node is for a naked single.</param>
/// <param name="Candidate">Indicates the target candidate.</param>
internal sealed record IttoryuPathNode(ref readonly Grid Grid, House House, Candidate Candidate) : IFormattable
{
	/// <summary>
	/// Indicates the target digit.
	/// </summary>
	public Digit Digit => Candidate % 9;

	/// <summary>
	/// Indicates the target cell.
	/// </summary>
	public Cell Cell => Candidate / 9;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Grid grid, out House house, out Cell cell, out Digit digit)
		=> ((grid, house, _), cell, digit) = (this, Candidate / 9, Candidate % 9);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		return House != -1
			? $"Full House / Hidden Single: {converter.CandidateConverter([Candidate])} in house {converter.HouseConverter(1 << House)}"
			: $"Naked Single: {converter.CandidateConverter([Candidate])}";
	}

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
