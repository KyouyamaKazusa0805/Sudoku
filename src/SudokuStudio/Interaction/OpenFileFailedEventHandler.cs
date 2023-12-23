namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="AnalyzePage.OpenFileFailed"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="AnalyzePage.OpenFileFailed"/>
public delegate void OpenFileFailedEventHandler(AnalyzePage sender, OpenFileFailedEventArgs e);
