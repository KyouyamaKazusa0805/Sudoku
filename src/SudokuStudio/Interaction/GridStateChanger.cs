namespace SudokuStudio.Interaction;

/// <summary>
/// Represents for a grid state changer callback method.
/// </summary>
/// <param name="grid">The grid to be changed.</param>
/// <param name="obj">An extra object to be used.</param>
internal delegate void GridStateChanger<T>(ref Grid grid, T obj) where T : allows ref struct;
