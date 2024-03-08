namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="CandidatePicker.SelectedCandidateChanged"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="CandidatePicker.SelectedCandidateChanged"/>
public delegate void CandidatePickerSelectedCandidateChangedEventHandler(CandidatePicker sender, CandidatePickerSelectedCandidateChangedEventArgs e);
