namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="DigitSetSelectedDigitChangedEventHandler"/>.
/// </summary>
/// <seealso cref="DigitSetSelectedDigitChangedEventHandler"/>
/// <param name="newDigit">Indicates the new digit selected.</param>
public sealed partial class DigitSetSelectedDigitChangedEventArgs([PrimaryConstructorParameter] Digit newDigit) : EventArgs;
