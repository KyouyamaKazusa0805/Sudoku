namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines a new page that stores a set of controls to analyze a sudoku grid.
/// </summary>
public sealed partial class AnalyzePage : Page
{
	/// <summary>
	/// Initializes an <see cref="AnalyzePage"/> instance.
	/// </summary>
	public AnalyzePage() => InitializeComponent();


	/// <inheritdoc/>
	protected override void OnKeyDown(KeyRoutedEventArgs e)
	{
		base.OnKeyDown(e);

		// This method routes the hotkeys.
		var modifierStatus = Keyboard.GetModifierStatusForCurrentThread();
		foreach (var ((modifiers, key), action) in new (Hotkey, Action)[]
		{
			(new(winsys::VirtualKeyModifiers.Control, winsys::VirtualKey.Z), SudokuPane.UndoStep),
			(new(winsys::VirtualKeyModifiers.Control, winsys::VirtualKey.Y), SudokuPane.RedoStep)
		})
		{
			if (modifierStatus == modifiers && e.Key == key)
			{
				action();
				break;
			}
		}

		e.Handled = false;
	}
}
