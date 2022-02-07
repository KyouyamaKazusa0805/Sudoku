namespace Sudoku.UI.Models;

/// <summary>
/// Defines a delegate type that holds a set of methods that is invoked when the corresponding
/// event is triggered.
/// </summary>
/// <param name="sender">The object that triggers the corresponding event.</param>
/// <param name="e">The data provided.</param>
public delegate void GridChangedEventHandler(object sender, GridChangedEventArgs e);