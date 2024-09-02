namespace SudokuStudio.Views.Pages.Settings.Basic;

/// <summary>
/// Represents notation setting page.
/// </summary>
public sealed partial class NotationSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="NotationSettingPage"/> instance.
	/// </summary>
	public NotationSettingPage()
	{
		InitializeComponent();
		InitializeControls();
	}


	/// <summary>
	/// Initializes for control properties.
	/// </summary>
	private void InitializeControls()
	{
		var isChinese = SR.IsChinese(CultureInfo.CurrentUICulture);
		Comma2ComboBoxItem_DefaultSeparator.Visibility = isChinese ? Visibility.Visible : Visibility.Collapsed;
		Comma2ComboBoxItem_DigitSeparator.Visibility = isChinese ? Visibility.Visible : Visibility.Collapsed;
	}


	private void ConceptNotationModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is Segmented { SelectedItem: SegmentedItem { Tag: int rawValue } })
		{
			Application.Current.AsApp().Preference.UIPreferences.ConceptNotationBasedKind = (CoordinateType)rawValue;
		}
	}

	private void NotationDefaultSeparatorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is Segmented { SelectedItem: SegmentedItem { Tag: string s } })
		{
			Application.Current.AsApp().Preference.UIPreferences.DefaultSeparatorInNotation = s;
		}
	}

	private void NotationDigitSeparatorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is Segmented { SelectedItem: SegmentedItem { Tag: string s } })
		{
			Application.Current.AsApp().Preference.UIPreferences.DefaultSeparatorInNotation = s;
		}
	}

	private void FinalRowLetterInK9NotationSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is Segmented { SelectedItem: SegmentedItem { Tag: string and [var ch] } })
		{
			Application.Current.AsApp().Preference.UIPreferences.FinalRowLetterInK9Notation = ch;
		}
	}
}
