namespace SudokuStudio.Views.Pages.Settings.Analysis;

/// <summary>
/// Represents baba grouping setting page.
/// </summary>
public sealed partial class BabaGroupingSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="BabaGroupingSettingPage"/> instance.
	/// </summary>
	public BabaGroupingSettingPage() => InitializeComponent();


	private void InitialLetterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var letter = (BabaGroupInitialLetter)((ComboBoxItem)((ComboBox)sender).SelectedItem).Tag;
		((App)Application.Current).Preference.AnalysisPreferences.InitialLetter = letter;
	}

	private void LetterCasingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var flag = (bool)((SegmentedItem)((Segmented)sender).SelectedItem).Tag;
		((App)Application.Current).Preference.AnalysisPreferences.LetterCasing = flag
			? BabaGroupLetterCasing.Upper
			: BabaGroupLetterCasing.Lower;
	}

	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		var analysisPref = ((App)Application.Current).Preference.AnalysisPreferences;
		InitialLetterComboBox.SelectedIndex = InitialLetterComboBox.Items
			.Select(valueSelector)
			.FirstOrDefault(p => p?.Control.Tag is BabaGroupInitialLetter letter && letter == analysisPref.InitialLetter)?
			.Index
			?? -1;
		LetterCasingComboBox.SelectedIndex = analysisPref.LetterCasing switch
		{
			BabaGroupLetterCasing.Upper => 0,
			BabaGroupLetterCasing.Lower => 1,
			_ => -1
		};


		static (ComboBoxItem Control, int Index)? valueSelector(object v, int i) => ((ComboBoxItem)v, i);
	}
}
