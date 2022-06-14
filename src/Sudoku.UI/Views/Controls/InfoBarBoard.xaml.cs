namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that handles the messages displaying via <see cref="InfoBar"/>s.
/// </summary>
/// <seealso cref="InfoBar"/>
public sealed partial class InfoBarBoard : UserControl, INotifyPropertyChanged, INotifyCollectionChanged
{
	/// <summary>
	/// The list of <see cref="InfoBarMessage"/>s.
	/// </summary>
	private readonly ObservableCollection<InfoBarMessage> _list = new();

	/// <summary>
	/// Indicates the backing field of property <see cref="InfoBarSpacing"/>.
	/// </summary>
	/// <seealso cref="InfoBarSpacing"/>
	private double _spacing;


	/// <summary>
	/// Initializes a <see cref="InfoBarBoard"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public InfoBarBoard() => InitializeComponent();


	/// <summary>
	/// Indicates whether the collection is not empty.
	/// </summary>
	public bool Any => _list.Count != 0;

	/// <summary>
	/// Indicates the spacing between two adjacent <see cref="InfoBar"/> instances.
	/// </summary>
	public double InfoBarSpacing
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _spacing;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_spacing = value;

			PropertyChanged?.Invoke(this, new(nameof(InfoBarSpacing)));
		}
	}


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <inheritdoc/>
	public event NotifyCollectionChangedEventHandler? CollectionChanged;


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
	/// with the specified <see cref="ManualSolverResult"/> instance.
	/// </summary>
	/// <param name="analysisResult">The <see cref="ManualSolverResult"/> instance.</param>
	/// <param name="severity">The severity. The default value is <see cref="InfoBarSeverity.Success"/>.</param>
	public void AddMessage(ManualSolverResult analysisResult, InfoBarSeverity severity = InfoBarSeverity.Success)
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
}
