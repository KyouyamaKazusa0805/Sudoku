namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="AnalyzePage.SaveFileFailed"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="AnalyzePage.SaveFileFailed"/>
public delegate void SaveFileFailedEventHandler(AnalyzePage sender, SaveFileFailedEventArgs e);
