namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="SudokuPane.Clicked"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="SudokuPane.Clicked"/>
public delegate void GridClickedEventHandler(SudokuPane sender, GridClickedEventArgs e);
