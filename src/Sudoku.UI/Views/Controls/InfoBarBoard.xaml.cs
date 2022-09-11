namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that handles the messages displaying via <see cref="InfoBar"/>s.
/// </summary>
/// <seealso cref="InfoBar"/>
public sealed partial class InfoBarBoard : UserControl, INotifyCollectionChanged
{
	/// <summary>
	/// The list of <see cref="InfoBarMessage"/>s.
	/// </summary>
	private readonly ObservableCollection<InfoBarMessage> _list = new();


	/// <summary>
	/// Initializes a <see cref="InfoBarBoard"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public InfoBarBoard() => InitializeComponent();


	/// <summary>
	/// Indicates whether the collection is not empty.
	/// </summary>
	public bool Any => _list.Count != 0;


	/// <inheritdoc/>
	public event NotifyCollectionChangedEventHandler? CollectionChanged;

	/// <summary>
	/// Triggers when the chosen step is changed.
	/// </summary>
	public event EventHandler<LogicalStep>? ChosenStepChanged;


	/// <summary>
	/// Creates a new <see cref="InfoBar"/> instance via the specified severity
	/// and the information.
	/// </summary>
	/// <param name="severity">The severity of the info bar.</param>
	/// <param name="info">The displaying text of the info bar.</param>
	/// <seealso cref="InfoBar"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMessage(InfoBarSeverity severity, string info)
	{
		var up = ((App)Application.Current).UserPreference;
		var a = _list.Prepend<InfoBarMessage, PlainMessage>;
		var b = _list.Add;
		(up.DescendingOrderedInfoBarBoard ? a : b)(new() { Severity = severity, Message = info });
	}

	/// <summary>
	/// Creates a new <see cref="InfoBar"/> instance via the specified severity,
	/// the information and the hyperlink button.
	/// </summary>
	/// <param name="severity">The severity of the info bar.</param>
	/// <param name="info">The displaying text of the info bar.</param>
	/// <param name="link">The hyperlink to be appended into.</param>
	/// <param name="linkDescription">The description of the hyperlink.</param>
	/// <seealso cref="InfoBar"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMessage(InfoBarSeverity severity, string info, string link, string linkDescription)
	{
		var up = ((App)Application.Current).UserPreference;
		var a = _list.Prepend<InfoBarMessage, HyperlinkMessage>;
		var b = _list.Add;
		(up.DescendingOrderedInfoBarBoard ? a : b)(
			new()
			{
				Severity = severity,
				Message = info,
				Hyperlink = new(link),
				HyperlinkDescription = linkDescription
			}
		);
	}

	/// <summary>
	/// Creates a new <see cref="InfoBar"/> instance via the specified severity,
	/// with the specified <see cref="LogicalSolverResult"/> instance.
	/// </summary>
	/// <param name="analysisResult">The <see cref="LogicalSolverResult"/> instance.</param>
	/// <param name="severity">The severity. The default value is <see cref="InfoBarSeverity.Success"/>.</param>
	public void AddMessage(LogicalSolverResult analysisResult, InfoBarSeverity severity = InfoBarSeverity.Success)
	{
		var up = ((App)Application.Current).UserPreference;
		var a = _list.Prepend<InfoBarMessage, ManualSolverResultMessage>;
		var b = _list.Add;
		(up.DescendingOrderedInfoBarBoard ? a : b)(
			new()
			{
				AnalysisResult = analysisResult,
				Severity = severity,
				Message = R["SudokuPage_InfoBar_AnalyzeSuccessfully"]!
			}
		);
	}

	/// <summary>
	/// Clears all messages.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearMessages() => _list.Clear();


	/// <summary>
	/// Triggers when the current control is loaded.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void UserControl_Loaded(object sender, RoutedEventArgs e) => _list.CollectionChanged += CollectionChanged;

	/// <summary>
	/// Triggers when the close button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void InfoBar_CloseButtonClick(InfoBar sender, object args)
	{
		// Property 'InfoBar.Tag' is special here: The property stores the base message model instance.
		// If we click the close button, the item should also be removed from the list '_list'.
		// Therefore, we should records the model instance here in order to get it, and then remove it.
		if (sender.Tag is not InfoBarMessage message)
		{
			return;
		}

		_list.Remove(message);
	}

	/// <summary>
	/// Triggers when the info bar is double tapped.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void InfoBar_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
	{
		if (sender is not InfoBar { Message: var message })
		{
			return;
		}

		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(message);
		Clipboard.SetContent(dataPackage);
	}

	/// <summary>
	/// Triggers when the item is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ListView_ItemClick(object sender, ItemClickEventArgs e)
		=> ChosenStepChanged?.Invoke(this, (LogicalStep)e.ClickedItem);
}
