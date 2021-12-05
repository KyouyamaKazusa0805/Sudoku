namespace Sudoku.Data;

/// <summary>
/// Defines a crosshatching line that used for displaying in a picture.
/// </summary>
/// <param name="Start">The start position.</param>
/// <param name="End">The end position.</param>
public readonly partial record struct Crosshatch(in Cells Start, in Cells End)
: IValueEquatable<Crosshatch>
, IJsonSerializable<Crosshatch, Crosshatch.JsonConverter>
{
	/// <inheritdoc/>
	public bool Equals(in Crosshatch other) => Start == other.Start && End == other.End;


	/// <summary>
	/// Implicit cast from <see cref="ValueTuple{T1, T2}"/> to <see cref="Crosshatch"/>.
	/// </summary>
	/// <param name="pair">The pair of crosshatching line data.</param>
	public static implicit operator Crosshatch(in (Cells Start, Cells End) pair) => new(pair.Start, pair.End);
}
