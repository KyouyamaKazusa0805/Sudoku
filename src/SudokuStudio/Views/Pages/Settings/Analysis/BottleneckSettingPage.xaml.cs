namespace SudokuStudio.Views.Pages.Settings.Analysis;

/// <summary>
/// Represents bottleneck setting page.
/// </summary>
public sealed partial class BottleneckSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="BottleneckSettingPage"/> instance.
	/// </summary>
	public BottleneckSettingPage() => InitializeComponent();


	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		var analysisPref = Application.Current.AsApp().Preference.AnalysisPreferences;
		DirectModeChooser.SelectedIndex = f(DirectModeChooser, analysisPref.DirectModeBottleneckType);
		PartialMarkingChooser.SelectedIndex = f(PartialMarkingChooser, analysisPref.PartialMarkingModeBottleneckType);
		FullMarkingChooser.SelectedIndex = f(FullMarkingChooser, analysisPref.FullMarkingModeBottleneckType);


		static int f(Segmented segmented, BottleneckType basic)
		{
			var c = segmented.ItemsPanelRoot.Children;
			for (var i = 0; i < c.Count; i++)
			{
				if (c[i] is SegmentedItem { Tag: BottleneckType t } && t == basic)
				{
					return i;
				}
			}
			return -1;
		}
	}

	private void DirectModeChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var tag = (BottleneckType)((SegmentedItem)DirectModeChooser.ItemsPanelRoot.Children[DirectModeChooser.SelectedIndex]).Tag;
		Application.Current.AsApp().Preference.AnalysisPreferences.DirectModeBottleneckType = tag;
	}

	private void PartialMarkingChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var tag = (BottleneckType)((SegmentedItem)PartialMarkingChooser.ItemsPanelRoot.Children[DirectModeChooser.SelectedIndex]).Tag;
		Application.Current.AsApp().Preference.AnalysisPreferences.PartialMarkingModeBottleneckType = tag;
	}

	private void FullMarkingChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var tag = (BottleneckType)((SegmentedItem)FullMarkingChooser.ItemsPanelRoot.Children[DirectModeChooser.SelectedIndex]).Tag;
		Application.Current.AsApp().Preference.AnalysisPreferences.FullMarkingModeBottleneckType = tag;
	}
}
