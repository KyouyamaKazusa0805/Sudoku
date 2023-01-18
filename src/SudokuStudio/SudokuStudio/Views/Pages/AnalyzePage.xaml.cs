namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines a new page that stores a set of controls to analyze a sudoku grid.
/// </summary>
public sealed partial class AnalyzePage : Page, INotifyPropertyChanged
{
	/// <summary>
	/// The default navigation transition instance that will create animation fallback while switching pages.
	/// </summary>
	private static readonly NavigationTransitionInfo NavigationTransitionInfo = new EntranceNavigationTransitionInfo();


	/// <summary>
	/// Indicates whether the generator is not running currently.
	/// </summary>
	[NotifyBackingField(Accessibility = GeneralizedAccessibility.Internal)]
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
			(new(winsys::VirtualKeyModifiers.Control, winsys::VirtualKey.Y), SudokuPane.RedoStep),
			(new(winsys::VirtualKeyModifiers.Control, winsys::VirtualKey.C), SudokuPane.Copy),
			(
				new(winsys::VirtualKeyModifiers.Control | winsys::VirtualKeyModifiers.Shift, winsys::VirtualKey.C),
				async () => await SudokuPane.CopySnapshotAsync()
			),
			(new(winsys::VirtualKeyModifiers.Control, winsys::VirtualKey.V), async () => await SudokuPane.PasteAsync())
		};

	/// <summary>
	/// An outer-layered method to switching pages. This method can be used by both
	/// <see cref="CommandBarView_ItemInvoked"/> and <see cref="CommandBarView_SelectionChanged"/>.
	/// </summary>
	/// <param name="container">The container.</param>
	/// <seealso cref="CommandBarView_ItemInvoked"/>
	/// <seealso cref="CommandBarView_SelectionChanged"/>
	private void SwitchingPage(NavigationViewItemBase container)
	{
		if (container == BasicOperationBar)
		{
			NavigateToPage(typeof(BasicOperation));
		}
	}

	/// <summary>
	/// Try to navigate to the target page.
	/// </summary>
	/// <param name="pageType">The target page type.</param>
	private void NavigateToPage(Type pageType)
	{
		if (CommandBarFrame.SourcePageType != pageType)
		{
			CommandBarFrame.Navigate(pageType, this, NavigationTransitionInfo);
		}
	}


	private void CommandBarView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		=> SwitchingPage(args.InvokedItemContainer);

	private void CommandBarView_Loaded(object sender, RoutedEventArgs e) => BasicOperationBar.IsSelected = true;

	private void CommandBarView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		=> SwitchingPage(args.SelectedItemContainer);

	private void CommandBarFrame_Navigated(object sender, NavigationEventArgs e)
	{
		switch (e)
		{
			case { Content: BasicOperation basicOperation, Parameter: AnalyzePage @this }:
			{
				basicOperation.BasePage = @this;
				break;
			}
		}
	}
}
