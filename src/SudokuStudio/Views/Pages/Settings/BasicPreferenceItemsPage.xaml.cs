using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.Text.Coordinate;
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
					SettingsCard_MakeLettersUpperCaseInRxCyNotation.IsEnabled = false;
					SettingsCard_MakeDigitBeforeCellInRxCyNotation.IsEnabled = false;
					SettingsCard_HouseNotationOnlyDisplayCapitalsInRxCyNotation.IsEnabled = false;
					SettingsCard_MakeLettersUpperCaseInK9Notation.IsEnabled = false;
					SettingsCard_FinalRowLetterInK9Notation.IsEnabled = false;
					SettingsCard_MakeLettersUpperCaseInExcelNotation.IsEnabled = false;
					break;
				}
				case ConceptNotationBased.RxCyBased:
				{
					SettingsCard_MakeLettersUpperCaseInRxCyNotation.IsEnabled = true;
					SettingsCard_MakeDigitBeforeCellInRxCyNotation.IsEnabled = true;
					SettingsCard_HouseNotationOnlyDisplayCapitalsInRxCyNotation.IsEnabled = true;
					SettingsCard_MakeLettersUpperCaseInK9Notation.IsEnabled = false;
					SettingsCard_FinalRowLetterInK9Notation.IsEnabled = false;
					SettingsCard_MakeLettersUpperCaseInExcelNotation.IsEnabled = false;
					break;
				}
				case ConceptNotationBased.K9Based:
				{
					SettingsCard_MakeLettersUpperCaseInRxCyNotation.IsEnabled = false;
					SettingsCard_MakeDigitBeforeCellInRxCyNotation.IsEnabled = false;
					SettingsCard_HouseNotationOnlyDisplayCapitalsInRxCyNotation.IsEnabled = false;
					SettingsCard_MakeLettersUpperCaseInK9Notation.IsEnabled = true;
					SettingsCard_FinalRowLetterInK9Notation.IsEnabled = true;
					SettingsCard_MakeLettersUpperCaseInExcelNotation.IsEnabled = false;
					break;
				}
				case ConceptNotationBased.ExcelBased:
				{
					SettingsCard_MakeLettersUpperCaseInRxCyNotation.IsEnabled = false;
					SettingsCard_MakeDigitBeforeCellInRxCyNotation.IsEnabled = false;
					SettingsCard_HouseNotationOnlyDisplayCapitalsInRxCyNotation.IsEnabled = false;
					SettingsCard_MakeLettersUpperCaseInK9Notation.IsEnabled = false;
					SettingsCard_FinalRowLetterInK9Notation.IsEnabled = false;
					SettingsCard_MakeLettersUpperCaseInExcelNotation.IsEnabled = true;
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
