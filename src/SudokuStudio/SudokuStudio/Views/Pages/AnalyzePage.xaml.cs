namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines a new page that stores a set of controls to analyze a sudoku grid.
/// </summary>
public sealed partial class AnalyzePage : Page, INotifyPropertyChanged
{
	/// <summary>
	/// Defines a default puzzle generator.
	/// </summary>
	private static readonly PatternBasedPuzzleGenerator Generator = new();


	/// <summary>
	/// Indicates whether the generator is not running currently.
	/// </summary>
	[NotifyBackingField]
	private bool _generatorIsNotRunning = true;

	/// <summary>
	/// Defines a key-value pair of functions that is used for routing hotkeys.
	/// </summary>
	private (Hotkey Hotkey, Action Action)[] _hotkeyFunctions;


	/// <summary>
	/// Initializes an <see cref="AnalyzePage"/> instance.
	/// </summary>
	public AnalyzePage()
	{
		InitializeComponent();
		InitializeField();
	}


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <inheritdoc/>
	protected override void OnKeyDown(KeyRoutedEventArgs e)
	{
		base.OnKeyDown(e);

		// This method routes the hotkeys.
		var modifierStatus = Keyboard.GetModifierStatusForCurrentThread();
		foreach (var ((modifiers, key), action) in _hotkeyFunctions)
		{
			if (modifierStatus == modifiers && e.Key == key)
			{
				action();
				break;
			}
		}

		e.Handled = false;
	}

	/// <summary>
	/// Try to initialize field <see cref="_hotkeyFunctions"/>.
	/// </summary>
	/// <seealso cref="_hotkeyFunctions"/>
	[MemberNotNull(nameof(_hotkeyFunctions))]
	private void InitializeField()
		=> _hotkeyFunctions = new (Hotkey, Action)[]
		{
			(new(winsys::VirtualKeyModifiers.Control, winsys::VirtualKey.Z), SudokuPane.UndoStep),
			(new(winsys::VirtualKeyModifiers.Control, winsys::VirtualKey.Y), SudokuPane.RedoStep)
		};


	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		GeneratorIsNotRunning = false;

		var grid = await Generator.GenerateAsync();

		GeneratorIsNotRunning = true;

		SudokuPane.Puzzle = grid;
	}
}
