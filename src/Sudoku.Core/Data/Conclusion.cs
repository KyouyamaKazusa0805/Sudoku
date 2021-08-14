namespace Sudoku.Data;

/// <summary>
/// Encapsulates a conclusion representation while solving in logic.
/// </summary>
/// <param name="ConclusionType">
/// The conclusion type to control the action of applying.
/// If the type is <see cref="ConclusionType.Assignment"/>,
/// this conclusion will be set value (Set a digit into a cell);
/// otherwise, a candidate will be removed.
/// </param>
/// <param name="Cell">Indicates the cell.</param>
/// <param name="Digit">Indicates the digit.</param>
/// <remarks>
/// Two <see cref="Conclusion"/>s can be compared with each other. If one of those two is an elimination
/// (i.e. holds the value <see cref="ConclusionType.Elimination"/> as the type), the instance
/// will be greater; if those two hold same conclusion type, but one of those two holds
/// the global index of the candidate position is greater, it is greater.
/// </remarks>
/// <seealso cref="ConclusionType.Elimination"/>
[AutoDeconstruct(nameof(ConclusionType), nameof(Candidate))]
public readonly partial record struct Conclusion(ConclusionType ConclusionType, int Cell, int Digit) : IValueEquatable<Conclusion>, IValueComparable<Conclusion>, IJsonSerializable<Conclusion, Conclusion.JsonConverter>
{
	/// <summary>
	/// Initializes an instance with a conclusion type and a candidate offset.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="candidate">The candidate offset.</param>
	public Conclusion(ConclusionType type, int candidate) : this(type, candidate / 9, candidate % 9)
	{
	}


	/// <summary>
	/// Indicates the candidate.
	/// </summary>
	public int Candidate => Cell * 9 + Digit;


	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	public void ApplyTo(ref SudokuGrid grid)
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

	/// <inheritdoc/>
	public bool Equals(in Conclusion other) =>
		Cell == other.Cell && Digit == other.Digit && ConclusionType == other.ConclusionType;

	/// <inheritdoc cref="object.GetHashCode"/>
	public override int GetHashCode() => ((int)ConclusionType + 1) * (Cell * 9 + Digit);

	/// <inheritdoc/>
	public int CompareTo(in Conclusion other) => GetHashCode() - other.GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() =>
		$@"r{(Cell / 9 + 1).ToString()}c{(Cell % 9 + 1).ToString()} {ConclusionType switch
		{
			ConclusionType.Assignment => "=",
			ConclusionType.Elimination => "<>"
		}} {(Digit + 1).ToString()}";


	/// <inheritdoc/>
	public static bool operator <(in Conclusion left, in Conclusion right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	public static bool operator <=(in Conclusion left, in Conclusion right) => left.CompareTo(right) <= 0;

	/// <inheritdoc/>
	public static bool operator >(in Conclusion left, in Conclusion right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	public static bool operator >=(in Conclusion left, in Conclusion right) => left.CompareTo(right) >= 0;
}
