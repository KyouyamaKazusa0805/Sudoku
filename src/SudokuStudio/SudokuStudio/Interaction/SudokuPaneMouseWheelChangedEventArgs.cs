namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="SudokuPaneMouseWheelChangedEventHandler"/>.
/// </summary>
/// <seealso cref="SudokuPaneMouseWheelChangedEventHandler"/>
public sealed class SudokuPaneMouseWheelChangedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="SudokuPaneMouseWheelChangedEventArgs"/> instance via a <see cref="bool"/> value
	/// indicating whether the mouse wheel is clockwise.
	/// </summary>
	/// <param name="isClockwise">A <see cref="bool"/> value indicating whether the mouse wheel is clockwise.</param>
	public SudokuPaneMouseWheelChangedEventArgs(bool isClockwise) => IsClockwise = isClockwise;


	/// <summary>
	/// Indicates whether the direction is clockwise.
	/// </summary>
	public bool IsClockwise { get; }
}
