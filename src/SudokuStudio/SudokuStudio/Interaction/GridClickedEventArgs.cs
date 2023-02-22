namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="GridUpdatedEventHandler"/>.
/// </summary>
/// <seealso cref="GridUpdatedEventHandler"/>
public sealed class GridClickedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="GridClickedEventArgs"/> instance.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	public GridClickedEventArgs(int candidate) => Candidate = candidate;


	/// <summary>
	/// Indicates the clicked candidate.
	/// </summary>
	public int Candidate { get; }

	/// <summary>
	/// Indicates the clicked cell.
	/// </summary>
	public int Cell => Candidate / 9;

	/// <summary>
	/// Indicates the clicked digit.
	/// </summary>
	public int Digit => Candidate % 9;

	/// <summary>
	/// Indicates the houses that the current cell lies in.
	/// </summary>
	public (int Block, int Row, int Column) Houses
	{
		get
		{
			scoped var result = (stackalloc int[3]);
			Cell.CopyHouseInfo(ref result[0]);

			return (result[0], result[1], result[2]);
		}
	}

	/// <summary>
	/// Indicates the chutes the current cell lines in.
	/// </summary>
	public (int Megarow, int Megacolumn) Chutes
		=> ((Cell.ToHouseIndex(HouseType.Row) - 9) / 3, (Cell.ToHouseIndex(HouseType.Column) - 18) / 3);
}
