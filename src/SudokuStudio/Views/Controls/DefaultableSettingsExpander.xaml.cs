using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using SudokuStudio.ComponentModel;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a <see cref="SettingsExpander"/> control that can revert the value to default.
/// </summary>
/// <seealso cref="SettingsExpander"/>
[DependencyProperty<object>("Header", IsNullable = true)]
[DependencyProperty<object>("Description", IsNullable = true)]
[DependencyProperty<object>("ItemsSource", IsNullable = true)]
[DependencyProperty<UIElement>("ItemsHeader", IsNullable = true)]
[DependencyProperty<UIElement>("ItemsFooter", IsNullable = true)]
[DependencyProperty<IconElement>("HeaderIcon", IsNullable = true)]
[DependencyProperty<IList<object>>("Items", IsNullable = true)]
public sealed partial class DefaultableSettingsExpander : UserControl
{
	/// <summary>
	/// Initializes a <see cref="DefaultableSettingsExpander"/> instance.
	/// </summary>
	public DefaultableSettingsExpander() => InitializeComponent();


	/// <inheritdoc cref="ButtonBase.Click"/>
	public event RoutedEventHandler? UndoButtonClick;


	private void Button_Click(object sender, RoutedEventArgs e) => UndoButtonClick?.Invoke(sender, e);
}
