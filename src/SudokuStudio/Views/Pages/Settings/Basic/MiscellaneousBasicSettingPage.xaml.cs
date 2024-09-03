namespace SudokuStudio.Views.Pages.Settings.Basic;

/// <summary>
/// Represents miscellaneous basic setting page.
/// </summary>
public sealed partial class MiscellaneousBasicSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="MiscellaneousBasicSettingPage"/> instance.
	/// </summary>
	public MiscellaneousBasicSettingPage() => InitializeComponent();


	private void PlaceholderTextSegmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is Segmented { SelectedItem: SegmentedItem { Tag: string and [var ch] } })
		{
			Application.Current.AsApp().Preference.UIPreferences.EmptyCellCharacter = ch;
		}
	}
}