using System.ComponentModel;
using static Sudoku.UI.ControlFactory;

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that handles the messages displaying via <see cref="InfoBar"/>s.
/// </summary>
public sealed partial class InfoBarBoard : UserControl, INotifyPropertyChanged
{
	/// <summary>
	/// Initializes a <see cref="InfoBarBoard"/> instance.
	/// </summary>
	public InfoBarBoard() => InitializeComponent();


	/// <summary>
	/// Indicates the number of messages existing in the board at present.
	/// </summary>
	[OneWayGetOnlyProperty]
	public int MessageCount => _cStackPanelDetails.Children.Count;


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// Creates a new <see cref="Microsoft.UI.Xaml.Controls.InfoBar"/> instance via the specified severity
	/// and the information.
	/// </summary>
	/// <param name="severity">The severity of the info bar.</param>
	/// <param name="info">The displaing text of the info bar.</param>
	/// <seealso cref="Microsoft.UI.Xaml.Controls.InfoBar"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMessage(InfoBarSeverity severity, string info)
	{
		var infoBar = InfoBar(severity);
		infoBar.Message = info;
		InfoBarAddHandlers(infoBar, _cStackPanelDetails);
		infoBar.IsOpen = true;

		PropertyChanged?.Invoke(this, new(nameof(MessageCount)));
	}

	/// <summary>
	/// Creates a new <see cref="Microsoft.UI.Xaml.Controls.InfoBar"/> instance via the specified severity,
	/// the information and the hyperlink button.
	/// </summary>
	/// <param name="severity">The severity of the info bar.</param>
	/// <param name="info">The displaing text of the info bar.</param>
	/// <param name="link">The hyperlink to be appended into.</param>
	/// <param name="linkDescription">The description of the hyperlink.</param>
	/// <seealso cref="Microsoft.UI.Xaml.Controls.InfoBar"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMessage(InfoBarSeverity severity, string info, string link, string linkDescription)
	{
		var infoBar = InfoBar(severity);
		infoBar.Message = info;
		infoBar.ActionButton = new HyperlinkButton { NavigateUri = new(link), Content = linkDescription };
		InfoBarAddHandlers(infoBar, _cStackPanelDetails);
		infoBar.IsOpen = true;

		PropertyChanged?.Invoke(this, new(nameof(MessageCount)));
	}

	/// <summary>
	/// Clears all messages.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearMessages()
	{
		if (_cStackPanelDetails.Children is [_, ..] children)
		{
			children.Clear();

			PropertyChanged?.Invoke(this, new(nameof(MessageCount)));
		}
	}

	/// <summary>
	/// Adds the specified <paramref name="infoBar"/> into the specified <paramref name="panel"/>
	/// as its child control, with the specified way to add.
	/// </summary>
	/// <param name="infoBar">
	/// The <see cref="InfoBar"/> instance to be added into the <paramref name="panel"/>.
	/// </param>
	/// <param name="panel">The panel to store the children controls.</param>
	/// <param name="insertAtFirstPlace">
	/// Indicates whether the control will be inserted at the first place into the parent panel.
	/// The default value is <see langword="true"/>.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InfoBarAddHandlers(InfoBar infoBar, StackPanel panel, bool insertAtFirstPlace = true)
	{
		((Action<StackPanel, InfoBar>)(insertAtFirstPlace ? insertion : appending))(panel, infoBar);

		infoBar.Closed += (s, e) => _ = e.Reason == InfoBarCloseReason.CloseButton && panel.Children.Remove(s);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void insertion(StackPanel panel, InfoBar infoBar) => panel.Children.Insert(0, infoBar);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void appending(StackPanel panel, InfoBar infoBar) => panel.Children.Add(infoBar);
	}
}
