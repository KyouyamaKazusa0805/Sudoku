namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="CandidatePickerSelectedCandidateChangedEventHandler"/>.
/// </summary>
/// <seealso cref="CandidatePickerSelectedCandidateChangedEventHandler"/>
/// <param name="newValue">The new value.</param>
public sealed partial class CandidatePickerSelectedCandidateChangedEventArgs([Property] Candidate newValue) : EventArgs;
