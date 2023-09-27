using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using SudokuStudio.ComponentModel;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a <see cref="SettingsCard"/> control that can revert the value to default.
/// </summary>
/// <seealso cref="SettingsCard"/>
[DependencyProperty<object>("Header", IsNullable = true)]
[DependencyProperty<object>("Description", IsNullable = true)]
[DependencyProperty<object>("DefaultValue", IsNullable = true, DocSummary = "Indicates the default value of the control that can be set. This value infects the control enability of undo button.")]
[DependencyProperty<ContentAlignment>("ContentAlignment")]
[DependencyProperty<IconElement>("ActionIcon", IsNullable = true)]
[DependencyProperty<IconElement>("HeaderIcon", IsNullable = true)]
public sealed partial class DefaultableSettingsCard : UserControl
{
	/// <summary>
	/// Initializes a <see cref="DefaultableSettingsCard"/> instance.
	/// </summary>
	public DefaultableSettingsCard() => InitializeComponent();


	/// <inheritdoc cref="ButtonBase.Click"/>
	public event RoutedEventHandler? UndoButtonClick;


	private void Button_Click(object sender, RoutedEventArgs e) => UndoButtonClick?.Invoke(sender, e);
}
