namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents a settings page that displays for basic preferences.
/// </summary>
public sealed partial class BasicPreferenceItemsPage : Page
{
	/// <summary>
	/// Initializes a <see cref="BasicPreferenceItemsPage"/> instance.
	/// </summary>
	public BasicPreferenceItemsPage()
	{
		InitializeComponent();
		InitializeControls();
	}


	/// <summary>
	/// Initializes for control properties.
	/// </summary>
	private void InitializeControls()
	{
		LanguageComboBox.SelectedIndex = ((App)Application.Current).Preference.UIPreferences.Language switch
		{
			0 => 0,
			1033 => 1,
			2052 => 2
		};
		Comma2ComboBoxItem_DefaultSeparator.Visibility = CultureInfo.CurrentUICulture.Name.Contains("zh") ? Visibility.Visible : Visibility.Collapsed;
		Comma2ComboBoxItem_DigitSeparator.Visibility = CultureInfo.CurrentUICulture.Name.Contains("zh") ? Visibility.Visible : Visibility.Collapsed;
	}

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

	private void ConceptNotationModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox { SelectedItem: ComboBoxItem { Tag: int rawValue } })
		{
			((App)Application.Current).Preference.UIPreferences.ConceptNotationBasedKind = (CoordinateType)rawValue;
		}
	}

	private void NotationDefaultSeparatorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox { SelectedItem: ComboBoxItem { Tag: string s } })
		{
			((App)Application.Current).Preference.UIPreferences.DefaultSeparatorInNotation = s;
		}
	}

	private void NotationDigitSeparatorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox { SelectedItem: ComboBoxItem { Tag: string s } })
		{
			((App)Application.Current).Preference.UIPreferences.DefaultSeparatorInNotation = s;
		}
	}

	private void FinalRowLetterInK9NotationSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox { SelectedItem: ComboBoxItem { Tag: string and [var ch] } })
		{
			((App)Application.Current).Preference.UIPreferences.FinalRowLetterInK9Notation = ch;
		}
	}

	private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> ((App)Application.Current).Preference.UIPreferences.Language = (int)((ComboBoxItem)LanguageComboBox.SelectedItem).Tag!;
}
