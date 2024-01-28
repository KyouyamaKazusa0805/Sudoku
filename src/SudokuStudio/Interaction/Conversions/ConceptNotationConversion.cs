namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about concept notation.
/// </summary>
internal static class ConceptNotationConversion
{
	public static int GetSelectedIndex(Segmented segmented)
	{
		var notationKind = ((App)Application.Current).Preference.UIPreferences.ConceptNotationBasedKind;
		var i = 0;
		foreach (var element in segmented.Items.Cast<SegmentedItem>())
		{
			if (element.Tag is int s && (CoordinateType)s == notationKind)
			{
				return i;
			}

			i++;
		}

		return -1;
	}

	public static int NotationSeparatorSelectorSelectedIndex(Segmented segmented)
	{
		var defaultSeparator = ((App)Application.Current).Preference.UIPreferences.DefaultSeparatorInNotation;
		var i = 0;
		foreach (var element in segmented.Items.Cast<SegmentedItem>())
		{
			if (element.Tag is string s && s == defaultSeparator)
			{
				return i;
			}

			i++;
		}

		return -1;
	}

	public static int FinalRowLetterInK9NotationSelectedIndex(Segmented segmented)
	{
		var @char = ((App)Application.Current).Preference.UIPreferences.FinalRowLetterInK9Notation;
		var i = 0;
		foreach (var element in segmented.Items.Cast<SegmentedItem>())
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
		=> (CoordinateType)((SegmentedItem)mode).Tag! == CoordinateType.RxCy;

	public static bool IsEnabled_SettingsCard_MakeDigitBeforeCellInRxCyNotation(object mode)
		=> (CoordinateType)((SegmentedItem)mode).Tag! == CoordinateType.RxCy;

	public static bool IsEnabled_SettingsCard_HouseNotationOnlyDisplayCapitalsInRxCyNotation(object mode)
		=> (CoordinateType)((SegmentedItem)mode).Tag! == CoordinateType.RxCy;

	public static bool IsEnabled_SettingsCard_MakeLettersUpperCaseInK9Notation(object mode)
		=> (CoordinateType)((SegmentedItem)mode).Tag! == CoordinateType.K9;

	public static bool IsEnabled_SettingsCard_FinalRowLetterInK9Notation(object mode)
		=> (CoordinateType)((SegmentedItem)mode).Tag! == CoordinateType.K9;

	public static bool IsEnabled_SettingsCard_MakeLettersUpperCaseInExcelNotation(object mode)
		=> (CoordinateType)((SegmentedItem)mode).Tag! == CoordinateType.Excel;
}
