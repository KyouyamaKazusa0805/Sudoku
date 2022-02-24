using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.UI.Data;

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that handles the messages displaying via <see cref="InfoBar"/>s.
/// </summary>
/// <seealso cref="InfoBar"/>
public sealed partial class InfoBarBoard : UserControl, INotifyPropertyChanged, INotifyCollectionChanged
{
	/// <summary>
	/// The list of <see cref="InfoBarInfo"/> or <see cref="InfoBarInfoWithLink"/>s.
	/// </summary>
	/// <seealso cref="InfoBarInfo"/>
	/// <seealso cref="InfoBarInfoWithLink"/>
	private readonly ObservableCollection<object> _list = new();

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
	/// <param name="info">The displaing text of the info bar.</param>
	/// <seealso cref="InfoBar"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMessage(InfoBarSeverity severity, string info) =>
		_list.Insert(0, new PlainInfoBarInfo { Severity = severity, Message = info });

	/// <summary>
	/// Creates a new <see cref="InfoBar"/> instance via the specified severity,
	/// the information and the hyperlink button.
	/// </summary>
	/// <param name="severity">The severity of the info bar.</param>
	/// <param name="info">The displaing text of the info bar.</param>
	/// <param name="link">The hyperlink to be appended into.</param>
	/// <param name="linkDescription">The description of the hyperlink.</param>
	/// <seealso cref="InfoBar"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMessage(InfoBarSeverity severity, string info, string link, string linkDescription) =>
		_list.Insert(
			0,
			new InfoBarInfoWithLink
			{
				Severity = severity,
				Message = info,
				Hyperlink = new(link),
				HyperlinkDescription = linkDescription
			}
		);

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
	private void UserControl_Loaded([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		_list.CollectionChanged += CollectionChanged;
}
