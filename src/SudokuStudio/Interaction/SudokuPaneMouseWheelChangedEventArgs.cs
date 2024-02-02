namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="SudokuPaneMouseWheelChangedEventHandler"/>.
/// </summary>
/// <param name="isClockwise">A <see cref="bool"/> value indicating whether the mouse wheel is clockwise.</param>
/// <remarks>
/// Initializes a <see cref="SudokuPaneMouseWheelChangedEventArgs"/> instance via a <see cref="bool"/> value
/// indicating whether the mouse wheel is clockwise.
/// </remarks>
/// <seealso cref="SudokuPaneMouseWheelChangedEventHandler"/>
public sealed partial class SudokuPaneMouseWheelChangedEventArgs([PrimaryCosntructorParameter] bool isClockwise) : EventArgs;
