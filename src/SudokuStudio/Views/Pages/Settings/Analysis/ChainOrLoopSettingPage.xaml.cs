namespace SudokuStudio.Views.Pages.Settings.Analysis;

/// <summary>
/// Represents chain or loop setting page.
/// </summary>
public sealed partial class ChainOrLoopSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="ChainOrLoopSettingPage"/> instance.
	/// </summary>
	public ChainOrLoopSettingPage() => InitializeComponent();


	/// <summary>
	/// Sets and updates the segmented item index.
	/// </summary>
	/// <param name="linkType">The link type.</param>
	/// <returns>The index.</returns>
	private int GetSegmentedItemIndex(LinkType linkType)
	{
		var analysisPref = Application.Current.AsApp().Preference.AnalysisPreferences;
		var dictionary = analysisPref.OverriddenLinkOptions;
		return (int)(dictionary.TryGetValue(linkType, out var option) ? option : LinkOption.House);
	}

	/// <summary>
	/// Sets and updates overridden link type value.
	/// </summary>
	/// <param name="linkType">The link type.</param>
	/// <param name="index">The index.</param>
	private void SetSegmentedItemIndex(LinkType linkType, int index)
	{
		if (index == -1)
		{
			return;
		}

		var analysisPref = Application.Current.AsApp().Preference.AnalysisPreferences;
		var dictionary = analysisPref.OverriddenLinkOptions;
		if (!dictionary.TryAdd(linkType, (LinkOption)index))
		{
			dictionary[linkType] = (LinkOption)index;
		}
	}


	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		SingleDigitComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.SingleDigit);
		SingleCellComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.SingleCell);
		LockedCandidatesComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.LockedCandidates);
		AlmostLockedSetsComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.AlmostLockedSets);
		AlmostUniqueRectangleComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.AlmostUniqueRectangle);
		AlmostAvoidableRectangleComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.AlmostAvoidableRectangle);
		KrakenNormalFishComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.KrakenNormalFish);
	}

	private void SingleDigitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.SingleDigit, SingleDigitComboBox.SelectedIndex);

	private void SingleCellComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.SingleCell, SingleCellComboBox.SelectedIndex);

	private void LockedCandidatesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.LockedCandidates, LockedCandidatesComboBox.SelectedIndex);

	private void AlmostLockedSetsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.AlmostLockedSets, AlmostLockedSetsComboBox.SelectedIndex);

	private void AlmostUniqueRectangleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.AlmostUniqueRectangle, AlmostUniqueRectangleComboBox.SelectedIndex);

	private void AlmostAvoidableRectangleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.AlmostAvoidableRectangle, AlmostAvoidableRectangleComboBox.SelectedIndex);

	private void KrakenNormalFishComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.KrakenNormalFish, KrakenNormalFishComboBox.SelectedIndex);
}
