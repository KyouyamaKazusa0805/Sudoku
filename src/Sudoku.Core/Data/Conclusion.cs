namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a conclusion representation while solving in logic.
	/// </summary>
	/// <remarks>
	/// Two <see cref="Conclusion"/>s can be compared with each other. If one of those two is an elimination
	/// (i.e. holds the value <see cref="ConclusionType.Elimination"/> as the type), the instance
	/// will be greater; if those two hold same conclusion type, but one of those two holds
	/// the global index of the candidate position is greater, it is greater.
	/// </remarks>
	/// <seealso cref="ConclusionType.Elimination"/>
	[AutoDeconstruct(nameof(ConclusionType), nameof(Candidate))]
	[AutoDeconstruct(nameof(ConclusionType), nameof(Cell), nameof(Digit))]
	[AutoEquality(nameof(ConclusionType), nameof(Cell), nameof(Digit))]
	public readonly partial struct Conclusion : IValueEquatable<Conclusion>, IValueComparable<Conclusion>, IJsonSerializable<Conclusion, Conclusion.JsonConverter>
	{
		/// <summary>
		/// Initializes an instance with a conclusion type, a cell offset and a digit.
		/// </summary>
		/// <param name="conclusionType">The conclusion type.</param>
		/// <param name="cell">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		public Conclusion(ConclusionType conclusionType, int cell, int digit)
		{
			ConclusionType = conclusionType;
			Cell = cell;
			Digit = digit;
		}

		/// <summary>
		/// Initializes an instance with a conclusion type and a candidate offset.
		/// </summary>
		/// <param name="conclusionType">The conclusion type.</param>
		/// <param name="candidate">The candidate offset.</param>
		public Conclusion(ConclusionType conclusionType, int candidate)
			: this(conclusionType, candidate / 9, candidate % 9)
		{
		}


		/// <summary>
		/// The cell offset.
		/// </summary>
		public int Cell { get; }

		/// <summary>
		/// The digit.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// Indicates the candidate.
		/// </summary>
		public int Candidate => Cell * 9 + Digit;

		/// <summary>
		/// The conclusion type to control the action of applying.
		/// If the type is <see cref="ConclusionType.Assignment"/>,
		/// this conclusion will be set value (Set a digit into a cell);
		/// otherwise, a candidate will be removed.
		/// </summary>
		public ConclusionType ConclusionType { get; }


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


		/// <summary>
		/// Determine whether the first instance is less than the second one.
		/// </summary>
		/// <param name="left">The first instance to compare.</param>
		/// <param name="right">The second instance to compare.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator <(in Conclusion left, in Conclusion right) => left.CompareTo(right) < 0;

		/// <summary>
		/// Determine whether the first instance is less than or equals to the second one.
		/// </summary>
		/// <param name="left">The first instance to compare.</param>
		/// <param name="right">The second instance to compare.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator <=(in Conclusion left, in Conclusion right) => left.CompareTo(right) <= 0;

		/// <summary>
		/// Determine whether the first instance is greater than the second one.
		/// </summary>
		/// <param name="left">The first instance to compare.</param>
		/// <param name="right">The second instance to compare.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator >(in Conclusion left, in Conclusion right) => left.CompareTo(right) > 0;

		/// <summary>
		/// Determine whether the first instance is greater than or equals to the second one.
		/// </summary>
		/// <param name="left">The first instance to compare.</param>
		/// <param name="right">The second instance to compare.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator >=(in Conclusion left, in Conclusion right) => left.CompareTo(right) >= 0;
	}
}
