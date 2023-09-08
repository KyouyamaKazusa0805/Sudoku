using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.Configuration;
using Windows.UI;

namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents a settings page that displays for basic preferences.
/// </summary>
public sealed partial class BasicPreferenceItemsPage : Page
{
	/// <summary>
	/// Initializes a <see cref="BasicPreferenceItemsPage"/> instance.
	/// </summary>
	public BasicPreferenceItemsPage() => InitializeComponent();


	private void BackdropSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox { SelectedItem: ComboBoxItem { Tag: string s } } && Enum.TryParse<BackdropKind>(s, out var value))
		{
			((App)Application.Current).Preference.UIPreferences.Backdrop = value;
		}
	}

	private void EmptyCellCharacterSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox { SelectedItem: ComboBoxItem { Tag: string and [var ch] } })
		{
			((App)Application.Current).Preference.UIPreferences.EmptyCellCharacter = ch;
		}
	}

	private void HouseCompletedFeedbackColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.HouseCompletedFeedbackColor = e;
}
