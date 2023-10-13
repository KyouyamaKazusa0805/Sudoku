using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.Analytics.Categorization;
using SudokuStudio.BindableSource;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents for a technique view.
/// </summary>
[DependencyProperty<double>("HorizontalSpacing", DocSummary = "Indicates the horizontal spacing.")]
[DependencyProperty<double>("VerticalSpacing", DocSummary = "Indicates the vertical spacing.")]
[DependencyProperty<TechniqueViewSelectionMode>("SelectionMode", DefaultValue = TechniqueViewSelectionMode.Single, DocSummary = "Indicates the selection mode.")]
[DependencyProperty<TechniqueSet>("SelectedTechniques", DocSummary = "Indicates the final selected techniques.")]
public sealed partial class TechniqueView : UserControl
{
	[Default]
	private static readonly TechniqueSet SelectedTechniquesDefaultValue = new();


	/// <summary>
	/// Indicates the internal token views.
	/// </summary>
	private readonly List<TokenView> _tokenViews = [];


	/// <summary>
	/// Initializes a <see cref="TechniqueView"/> instance.
	/// </summary>
	public TechniqueView() => InitializeComponent();


	/// <summary>
	/// The items source.
	/// </summary>
	private TechniqueViewGroupBindableSource[] ItemsSource
		=> [
			..
			from technique in Enum.GetValues<Technique>()[1..]
			where !technique.GetFeature().Flags(TechniqueFeature.NotImplemented)
			select new TechniqueViewBindableSource(technique) into item
			group item by item.ContainingGroup into itemGroup
			orderby itemGroup.Key
			select new TechniqueViewGroupBindableSource(itemGroup.Key, [.. itemGroup])
		];

	/// <summary>
	/// The entry that can traverse for all tokens.
	/// </summary>
	private Dictionary<Technique, TokenItem> TokenItems
		=> new([
			..
			from view in _tokenViews
			from item in view.Items
			let tokenItem = item as TokenItem
			where tokenItem is not null
			let tag = (Technique)tokenItem.Tag!
			select new KeyValuePair<Technique, TokenItem>(tag, tokenItem)
		]);


	[Callback]
	private static void SelectedTechniquesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (TechniqueView view, { NewValue: TechniqueSet set }))
		{
			foreach (var (technique, tokenItem) in view.TokenItems)
			{
				if (set.Contains(technique))
				{
					tokenItem.IsSelected = true;
				}
			}
		}
	}

	private void TokenView_Loaded(object sender, RoutedEventArgs e) => _tokenViews.Add((TokenView)sender);

	private void TokenView_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e is
			{
				OriginalSource: TokenItem { Tag: Technique field, IsSelected: var isSelected },
				ClickedItem: TechniqueViewBindableSource
			})
		{
			switch (isSelected)
			{
				case true when !SelectedTechniques.Contains(field):
				{
					SelectedTechniques.Add(field);
					break;
				}
				case false when SelectedTechniques.Contains(field):
				{
					SelectedTechniques.Remove(field);
					break;
				}
			}
		}
	}
}
