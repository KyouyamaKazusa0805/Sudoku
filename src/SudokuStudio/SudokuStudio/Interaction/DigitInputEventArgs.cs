namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="DigitInputEventHandler"/>.
/// </summary>
/// <seealso cref="DigitInputEventHandler"/>
public sealed class DigitInputEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="DigitInputEventArgs"/> instance via the specified cell and input digit.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="newDigitInput">The digit input.</param>
	public DigitInputEventArgs(int cell, int newDigitInput) => (Cell, DigitInput) = (cell, newDigitInput);


	/// <summary>
	/// Indicates the cell that raises the event triggered.
	/// </summary>
	public int Cell { get; }

	/// <summary>
	/// Indicates the digit input that raises the event triggered. -1 is for clear the cell.
	/// </summary>
	public int DigitInput { get; }

	/// <summary>
	/// Indicates the candidate constructed. -1 is for the case that <see cref="DigitInput"/> is -1.
	/// </summary>
	public int Candidate => DigitInput != -1 ? Cell * 9 + DigitInput : -1;
}
