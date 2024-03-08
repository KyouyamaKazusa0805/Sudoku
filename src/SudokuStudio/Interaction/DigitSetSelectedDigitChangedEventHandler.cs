namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="DigitSet.SelectedDigitChanged"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="DigitSet.SelectedDigitChanged"/>
public delegate void DigitSetSelectedDigitChangedEventHandler(DigitSet sender, DigitSetSelectedDigitChangedEventArgs e);
