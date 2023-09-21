using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.Text.Coordinate;
using SudokuStudio.Configuration;
using SudokuStudio.Rendering;
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
		if (sender is ComboBox { SelectedItem: ComboBoxItem { Tag: int s } })
		{
			var converted = (ConceptNotationBased)s;
			switch (((App)Application.Current).Preference.UIPreferences.ConceptNotationBasedKind = converted)
			{
				case ConceptNotationBased.LiteralBased:
				{
					SettingsCard_MakeLettersUpperCaseInRxCyNotation.Visibility = Visibility.Collapsed;
					SettingsCard_MakeDigitBeforeCellInRxCyNotation.Visibility = Visibility.Collapsed;
					SettingsCard_HouseNotationOnlyDisplayCapitalsInRxCyNotation.Visibility = Visibility.Collapsed;
					SettingsCard_MakeLettersUpperCaseInK9Notation.Visibility = Visibility.Collapsed;
					SettingsCard_FinalRowLetterInK9Notation.Visibility = Visibility.Collapsed;
					break;
				}
				case ConceptNotationBased.RxCyBased:
				{
					SettingsCard_MakeLettersUpperCaseInRxCyNotation.Visibility = Visibility.Visible;
					SettingsCard_MakeDigitBeforeCellInRxCyNotation.Visibility = Visibility.Visible;
					SettingsCard_HouseNotationOnlyDisplayCapitalsInRxCyNotation.Visibility = Visibility.Visible;
					SettingsCard_MakeLettersUpperCaseInK9Notation.Visibility = Visibility.Collapsed;
					SettingsCard_FinalRowLetterInK9Notation.Visibility = Visibility.Collapsed;
					break;
				}
				case ConceptNotationBased.K9Based:
				{
					SettingsCard_MakeLettersUpperCaseInRxCyNotation.Visibility = Visibility.Collapsed;
					SettingsCard_MakeDigitBeforeCellInRxCyNotation.Visibility = Visibility.Collapsed;
					SettingsCard_HouseNotationOnlyDisplayCapitalsInRxCyNotation.Visibility = Visibility.Collapsed;
					SettingsCard_MakeLettersUpperCaseInK9Notation.Visibility = Visibility.Visible;
					SettingsCard_FinalRowLetterInK9Notation.Visibility = Visibility.Visible;
					break;
				}
			}
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
}
