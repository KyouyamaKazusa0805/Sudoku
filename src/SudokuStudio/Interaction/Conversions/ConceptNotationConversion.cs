using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.Text.Coordinate;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about concept notation.
/// </summary>
internal static class ConceptNotationConversion
{
	public static int GetSelectedIndex(ComboBox comboBox)
	{
		var notationKind = ((App)Application.Current).Preference.UIPreferences.ConceptNotationBasedKind;
		var i = 0;
		foreach (var element in comboBox.Items.Cast<ComboBoxItem>())
		{
			if (element.Tag is int s && (ConceptNotationBased)s == notationKind)
			{
				return i;
			}

			i++;
		}

		return -1;
	}

	public static int NotationSeparatorSelectorSelectedIndex(ComboBox comboBox)
	{
		var defaultSeparator = ((App)Application.Current).Preference.UIPreferences.DefaultSeparatorInNotation;
		var i = 0;
		foreach (var element in comboBox.Items.Cast<ComboBoxItem>())
		{
			if (element.Tag is string s && s == defaultSeparator)
			{
				return i;
			}

			i++;
		}

		return -1;
	}

	public static int FinalRowLetterInK9NotationSelectedIndex(ComboBox comboBox)
	{
		var @char = ((App)Application.Current).Preference.UIPreferences.FinalRowLetterInK9Notation;
		var i = 0;
		foreach (var element in comboBox.Items.Cast<ComboBoxItem>())
		{
			if (element.Tag is string and [var ch] && char.ToLower(ch) == char.ToLower(@char))
			{
				return i;
			}

			i++;
		}

		return -1;
	}

	public static bool IsEnabled_SettingsCard_MakeLettersUpperCaseInRxCyNotation(object mode)
		=> (ConceptNotationBased)((ComboBoxItem)mode).Tag! == ConceptNotationBased.RxCyBased;

	public static bool IsEnabled_SettingsCard_MakeDigitBeforeCellInRxCyNotation(object mode)
		=> (ConceptNotationBased)((ComboBoxItem)mode).Tag! == ConceptNotationBased.RxCyBased;

	public static bool IsEnabled_SettingsCard_HouseNotationOnlyDisplayCapitalsInRxCyNotation(object mode)
		=> (ConceptNotationBased)((ComboBoxItem)mode).Tag! == ConceptNotationBased.RxCyBased;

	public static bool IsEnabled_SettingsCard_MakeLettersUpperCaseInK9Notation(object mode)
		=> (ConceptNotationBased)((ComboBoxItem)mode).Tag! == ConceptNotationBased.K9Based;

	public static bool IsEnabled_SettingsCard_FinalRowLetterInK9Notation(object mode)
		=> (ConceptNotationBased)((ComboBoxItem)mode).Tag! == ConceptNotationBased.K9Based;

	public static bool IsEnabled_SettingsCard_MakeLettersUpperCaseInExcelNotation(object mode)
		=> (ConceptNotationBased)((ComboBoxItem)mode).Tag! == ConceptNotationBased.ExcelBased;
}
