namespace SudokuStudio.Views.Pages.Settings.Analysis;

/// <summary>
/// Represents chain setting page.
/// </summary>
public sealed partial class ChainSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="ChainSettingPage"/> instance.
	/// </summary>
	public ChainSettingPage() => InitializeComponent();


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
		UniqueRectangleSameDigitComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.UniqueRectangle_SameDigit);
		UniqueRectangleDifferentDigitComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.UniqueRectangle_DifferentDigit);
		UniqueRectangleSingleSideExternalComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.UniqueRectangle_SingleSideExternal);
		UniqueRectangleDoubleSideExternalComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.UniqueRectangle_DoubleSideExternal);
		AvoidableRectangleComboBox.SelectedIndex = GetSegmentedItemIndex(LinkType.AvoidableRectangle);
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

	private void UniqueRectangleSameDigitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.UniqueRectangle_SameDigit, UniqueRectangleSameDigitComboBox.SelectedIndex);

	private void UniqueRectangleDoubleSideExternalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.UniqueRectangle_DifferentDigit, UniqueRectangleDifferentDigitComboBox.SelectedIndex);

	private void UniqueRectangleSingleSideExternalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.UniqueRectangle_SingleSideExternal, UniqueRectangleSingleSideExternalComboBox.SelectedIndex);

	private void UniqueRectangleDifferentDigitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.UniqueRectangle_DoubleSideExternal, UniqueRectangleDoubleSideExternalComboBox.SelectedIndex);

	private void AvoidableRectangleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.AvoidableRectangle, AvoidableRectangleComboBox.SelectedIndex);

	private void KrakenNormalFishComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> SetSegmentedItemIndex(LinkType.KrakenNormalFish, KrakenNormalFishComboBox.SelectedIndex);
}
