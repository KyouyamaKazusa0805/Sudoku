namespace SudokuStudio.Collection;

/// <summary>
/// Provides with an event handler delegate type triggered when the target collection is changed.
/// </summary>
/// <typeparam name="T">The type of each element stored in the collection.</typeparam>
/// <param name="sender">The collection.</param>
public delegate void ObservableStackChangedEventHandler<T>(ObservableStack<T> sender);
