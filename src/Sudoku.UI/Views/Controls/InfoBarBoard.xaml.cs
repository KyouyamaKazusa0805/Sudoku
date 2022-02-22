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
		InfoBar(severity)
			.WithMessage(info)
			.WithParentPanel(_cStackPanelDetails)
			.Open();

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
		InfoBar(severity)
			.WithMessage(info)
			.WithParentPanel(_cStackPanelDetails)
			.WithLinkButton(link, linkDescription)
			.Open();

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
}
