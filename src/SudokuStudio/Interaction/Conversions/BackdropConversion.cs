using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.Configuration;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about backdrop.
/// </summary>
internal static class BackdropConversion
{
	public static Offset GetSelectedIndex(ComboBox comboBox)
	{
		var backdropKind = ((App)Application.Current).Preference.UIPreferences.Backdrop;
		var i = 0;
		foreach (var element in comboBox.Items.Cast<ComboBoxItem>())
		{
			if (element.Tag is string s && Enum.TryParse<BackdropKind>(s, out var target) && target == backdropKind)
			{
				return i;
			}

			i++;
		}

		return -1;
	}
}
