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
	private static readonly TechniqueSet SelectedTechniquesDefaultValue = [];


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
			from item in view.ItemsPanelRoot.Children
			let tokenItem = item as TokenItem
			where tokenItem is not null
			let content = (TechniqueViewBindableSource)tokenItem.Content
			select new KeyValuePair<Technique, TokenItem>(content.TechniqueField, tokenItem)
		]);


	/// <summary>
	/// Indicates the event triggered when selected techniques property is changed.
	/// </summary>
	public event TechniqueViewSelectedTechniquesChangedEventHandler? SelectedTechniquesChanged;

	/// <summary>
	/// Indicates the event triggered when the current selected technique is changed.
	/// </summary>
	public event TechniqueViewCurrentSelectedTechniqueChangedEventHandler? CurrentSelectedTechniqueChanged;


	/// <summary>
	/// Try to update all token items via selection state.
	/// </summary>
	private void UpdateSelection(TechniqueSet set)
	{
		foreach (var (technique, item) in TokenItems)
		{
			item.IsSelected = set.Contains(technique);
		}
	}


	[Callback]
	private static void SelectedTechniquesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (TechniqueView view, { NewValue: TechniqueSet set }))
		{
			view.UpdateSelection(set);
		}
	}

	[Callback]
	private static void SelectionModePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (TechniqueView view, { NewValue: TechniqueViewSelectionMode mode }))
		{
			foreach (var tokenView in view._tokenViews)
			{
				tokenView.SelectionMode = mode switch
				{
					TechniqueViewSelectionMode.None => ListViewSelectionMode.None,
					TechniqueViewSelectionMode.Single => ListViewSelectionMode.Single,
					TechniqueViewSelectionMode.Multiple => ListViewSelectionMode.Multiple
				};
			}
		}
	}


	private void TokenView_Loaded(object sender, RoutedEventArgs e)
	{
		var p = (TokenView)sender;
		p.SelectionMode = SelectionMode switch
		{
			TechniqueViewSelectionMode.None => ListViewSelectionMode.None,
			TechniqueViewSelectionMode.Single => ListViewSelectionMode.Single,
			TechniqueViewSelectionMode.Multiple => ListViewSelectionMode.Multiple
		};

		_tokenViews.Add(p);

		if (_tokenViews.Count == ItemsSource.Length)
		{
			UpdateSelection(SelectedTechniques);
		}
	}

	private void TokenView_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e is
			{
				OriginalSource: TokenView { ItemsPanelRoot.Children: var children } p,
				ClickedItem: TechniqueViewBindableSource { TechniqueField: var field }
			}
			&& children.OfType<TokenItem>().FirstOrDefault(s => lambda(s, field)) is { IsSelected: var isSelected } child)
		{
			var add = SelectedTechniques.Add;
			var remove = SelectedTechniques.Remove;
			(isSelected ? remove : add)(field);

			CurrentSelectedTechniqueChanged?.Invoke(this, new(field, isSelected));
			SelectedTechniquesChanged?.Invoke(this, new(SelectedTechniques));

			if (SelectionMode == TechniqueViewSelectionMode.Single)
			{
				// Special case: If the selection mode is "Single", we should remove all the other enabled token items.
				foreach (var q in _tokenViews)
				{
					if (!ReferenceEquals(p, q))
					{
						foreach (var element in q.ItemsPanelRoot.Children.OfType<TokenItem>())
						{
							if (element.IsSelected)
							{
								element.IsSelected = false;
							}
						}
					}
				}
			}
		}


		static bool lambda(TokenItem s, Technique field) => s.Content is TechniqueViewBindableSource { TechniqueField: var f } && f == field;
	}
}
