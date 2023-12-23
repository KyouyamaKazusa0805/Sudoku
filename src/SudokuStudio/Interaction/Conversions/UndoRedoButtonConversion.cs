namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about undo and redo buttons.
/// </summary>
internal static class UndoRedoButtonConversion
{
	public static bool GetButtonIsEnabled(ObservableStack<Grid> stack) => stack.Count != 0;
}
