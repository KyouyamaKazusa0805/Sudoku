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
	/// <param name="mouseButton">Indicates the mouse button clicked.</param>
	/// <param name="candidate">The candidate.</param>
	public GridClickedEventArgs(MouseButton mouseButton, Candidate candidate) => (MouseButton, Candidate) = (mouseButton, candidate);


	/// <summary>
	/// Indicates the clicked candidate.
	/// </summary>
	public Candidate Candidate { get; }

	/// <summary>
	/// Indicates the clicked cell.
	/// </summary>
	public Cell Cell => Candidate / 9;

	/// <summary>
	/// Indicates the clicked digit.
	/// </summary>
	public Digit Digit => Candidate % 9;

	/// <summary>
	/// Indicates the houses that the current cell lies in.
	/// </summary>
	public (House Block, House Row, House Column) Houses => GetHouse(Candidate);

	/// <summary>
	/// Indicates the chutes the current cell lines in.
	/// </summary>
	public (int Megarow, int Megacolumn) Chutes => GetChute(Candidate);

	/// <summary>
	/// Indicates the clicked mouse button.
	/// </summary>
	public MouseButton MouseButton { get; }


	/// <summary>
	/// Try to calculate house indices for a candiate.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <returns>
	/// A triplet indicating block, row and column index. All three values are <![CDATA[>= 0 and < 9]]>.
	/// </returns>
	public static (House Block, House Row, House Column) GetHouse(Candidate candidate)
	{
		var cell = candidate / 9;

		scoped var result = (stackalloc House[3]);
		cell.CopyHouseInfo(ref result[0]);

		return (result[0], result[1], result[2]);
	}

	/// <summary>
	/// Try to calculate chute indices for a candidate.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <returns>A pair of two values indicating chute indices in a mega-row and mega-column.</returns>
	public static (int Megarow, int Megacolumn) GetChute(Candidate candidate)
	{
		var cell = candidate / 9;
		return ((cell.ToHouseIndex(HouseType.Row) - 9) / 3, (cell.ToHouseIndex(HouseType.Column) - 18) / 3);
	}
}
