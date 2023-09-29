using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Sudoku.Analytics.Categorization;
using SudokuStudio.BindableSource;
using SudokuStudio.ComponentModel;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a selector that can select a list of <see cref="Technique"/> instances.
/// </summary>
/// <remarks>
/// Partially referenced the code from those links:
/// <list type="bullet">
/// <item>
/// <see href="https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/ControlPages/AnnotatedScrollBarPage.xaml">XAML</see>
/// </item>
/// <item>
/// <see href="https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/ControlPages/AnnotatedScrollBarPage.xaml.cs">C#</see>
/// </item>
/// </list>
/// </remarks>
[DependencyProperty<TechniqueSet>("SelectedTechniques", DocSummary = "Indicates the selected techniques.")]
public sealed partial class TechniqueSetSelector : UserControl
{
	/// <summary>
	/// Indicates the width of each item. The value should be strictly equal to the target value assigned in XAML
	/// (i.e. base width + uniformed margin).
	/// </summary>
	private const int ItemWidth = 200;

	/// <summary>
	/// Indicates the height of each item. The value should be strictly equal to the target value assigned in XAML
	/// (i.e. base height + uniformed margin)L.
	/// </summary>
	private const int ItemHeight = 40;


	/// <summary>
	/// The items source.
	/// </summary>
	internal TechniqueSetTechniqueBindableSource[] ItemsSource
		=>
		from technique in Enum.GetValues<Technique>()
		where !technique.GetFeature().Flags(TechniqueFeature.NotImplemented)
		select new TechniqueSetTechniqueBindableSource { TechniqueField = technique };


	/// <summary>
	/// Initializes a <see cref="TechniqueSetSelector"/> instance.
	/// </summary>
	public TechniqueSetSelector()
	{
		InitializeComponent();

		InitializeLabels();
	}


	/// <summary>
	/// Initalizes for labels.
	/// </summary>
	private void InitializeLabels()
	{
		if (ScrollBar is not null)
		{
			ScrollBar.Labels.Clear();

			var baseCount = 0;
			foreach (var (group, techniquesInThisGroup) in TechniqueSet.TechniqueRelationGroups)
			{
				ScrollBar.Labels.Add(new(group.GetShortenedName(), GetOffsetOfItem(baseCount)));

				baseCount += techniquesInThisGroup.Count;
			}
		}
	}

	/// <summary>
	/// Calculates the number of items in a same row via each item width.
	/// </summary>
	/// <returns>The number of items.</returns>
	private int GetItemsPerRow() => ToggleButtonsDisplayer is { ActualWidth: var w and not 0 } ? (int)Math.Max(w / ItemWidth, 1) : 1;

	/// <summary>
	/// Calculates the offset value of the item at the specified index.
	/// </summary>
	/// <param name="itemIndex">The index of the desired item.</param>
	/// <returns>The offset value.</returns>
	private int GetOffsetOfItem(int itemIndex) => ItemHeight * (itemIndex / GetItemsPerRow());

	/// <summary>
	/// Gets the label name via the specified offset.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <returns>The name of the label.</returns>
	private string GetOffsetLabel(double offset)
	{
		var baseCount = 0;
		foreach (var (group, techniquesInThisGroup) in TechniqueSet.TechniqueRelationGroups)
		{
			baseCount += techniquesInThisGroup.Count;
			if (offset <= GetOffsetOfItem(baseCount - 1))
			{
				return group.GetShortenedName();
			}
		}

		return TechniqueGroup.BruteForce.GetShortenedName();
	}


	private void ToggleButton_Checked(object sender, RoutedEventArgs e)
	{
		if (sender is ToggleButton { Tag: Technique field })
		{
			SelectedTechniques.Add(field);
		}
	}

	private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
	{
		if (sender is ToggleButton { Tag: Technique field })
		{
			SelectedTechniques.Remove(field);
		}
	}

	private void ScrollBar_DetailLabelRequested(AnnotatedScrollBar sender, AnnotatedScrollBarDetailLabelRequestedEventArgs args)
		=> args.Content = GetOffsetLabel(args.ScrollOffset);
}
