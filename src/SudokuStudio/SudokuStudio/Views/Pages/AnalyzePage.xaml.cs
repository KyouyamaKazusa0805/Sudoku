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
#if DEBUG
		if (Array.Exists(Enum.GetValues<winsys::VirtualKeyModifiers>(), element => Array.IndexOf(toKeys(element), e.Key) != -1))
		{
			goto RouteToInnerControls;
		}
#endif

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

#if DEBUG
	RouteToInnerControls:
#endif
		e.Handled = false;


#if DEBUG
		static winsys::VirtualKey[] toKeys(winsys::VirtualKeyModifiers modifier)
			=> modifier switch
			{
				winsys::VirtualKeyModifiers.Control => new[] { winsys::VirtualKey.Control },
				winsys::VirtualKeyModifiers.Shift => new[] { winsys::VirtualKey.Shift },
				winsys::VirtualKeyModifiers.Menu => new[] { winsys::VirtualKey.Menu },
				winsys::VirtualKeyModifiers.Windows => new[] { winsys::VirtualKey.LeftWindows, winsys::VirtualKey.RightWindows },
				_ => Array.Empty<winsys::VirtualKey>()
			};
#endif
	}
}
