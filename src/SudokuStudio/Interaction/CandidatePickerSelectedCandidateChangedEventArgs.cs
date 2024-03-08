namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="CandidatePickerSelectedCandidateChangedEventHandler"/>.
/// </summary>
/// <seealso cref="CandidatePickerSelectedCandidateChangedEventHandler"/>
/// <param name="newValue">The new value.</param>
/// <remarks>
/// Initializes an <see cref="OpenFileFailedEventArgs"/> instance via the specified reason.
/// </remarks>
public sealed partial class CandidatePickerSelectedCandidateChangedEventArgs([PrimaryConstructorParameter] Candidate newValue) : EventArgs;
