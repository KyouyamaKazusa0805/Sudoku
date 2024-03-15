namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a technique information modifier page.
/// </summary>
[DependencyProperty<TechniqueGroup>("CurrentTechniqueGroup", Accessibility = Accessibility.Internal)]
public sealed partial class TechniqueInfoModifierPage : Page
{
	/// <summary>
	/// Indicates the default grid row height.
	/// </summary>
	private static readonly GridLength DefaultHeight = new(50, GridUnitType.Pixel);


	/// <summary>
	/// Initializes a <see cref="TechniqueInfoModifierPage"/> instance.
	/// </summary>
	public TechniqueInfoModifierPage() => InitializeComponent();


	[Callback]
	private static void CurrentTechniqueGroupPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (TechniqueInfoModifierPage p, { NewValue: TechniqueGroup techniqueGroup }))
		{
			return;
		}

		var values = techniqueGroup.GetTechniques();
		var g = p.MainGrid;
		setRowDefinitions(g, values);

		// Add for table title.
		addTableTitleRow(g);

		// Add for children controls.
		var pref = ((App)Application.Current).Preference.TechniqueInfoPreferences;
		for (var (i, j) = (1, 0); j < values.Count; i++, j++)
		{
			var technique = values[j];
			var name = technique.GetName(App.CurrentCulture);
			var englishName = technique.GetEnglishName();

			//
			// Name
			//
			var nameControl = new TextBlock
			{
				Text = name,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new(6, 0, 0, 0)
			};
			GridLayout.SetRow(nameControl, i);
			GridLayout.SetColumn(nameControl, 0);

			//
			// English name
			//
			var englishNameControl = new TextBlock { Text = englishName, VerticalAlignment = VerticalAlignment.Center };
			GridLayout.SetRow(englishNameControl, i);
			GridLayout.SetColumn(englishNameControl, 1);

			//
			// Difficulty level
			//
			var difficultyLevelControl = new Segmented
			{
				SelectionMode = ListViewSelectionMode.Single,
				Items =
				{
					new SegmentedItem
					{
						Content = ResourceDictionary.Get("DifficultyLevel_Easy", App.CurrentCulture),
						Foreground = DifficultyLevelConversion.GetForegroundColor(DifficultyLevel.Easy),
						Background = DifficultyLevelConversion.GetBackgroundColor(DifficultyLevel.Easy),
						Tag = DifficultyLevel.Easy
					},
					new SegmentedItem
					{
						Content = ResourceDictionary.Get("DifficultyLevel_Moderate", App.CurrentCulture),
						Foreground = DifficultyLevelConversion.GetForegroundColor(DifficultyLevel.Moderate),
						Background = DifficultyLevelConversion.GetBackgroundColor(DifficultyLevel.Moderate),
						Tag = DifficultyLevel.Moderate
					},
					new SegmentedItem
					{
						Content = ResourceDictionary.Get("DifficultyLevel_Hard", App.CurrentCulture),
						Foreground = DifficultyLevelConversion.GetForegroundColor(DifficultyLevel.Hard),
						Background = DifficultyLevelConversion.GetBackgroundColor(DifficultyLevel.Hard),
						Tag = DifficultyLevel.Hard
					},
					new SegmentedItem
					{
						Content = ResourceDictionary.Get("DifficultyLevel_Fiendish", App.CurrentCulture),
						Foreground = DifficultyLevelConversion.GetForegroundColor(DifficultyLevel.Fiendish),
						Background = DifficultyLevelConversion.GetBackgroundColor(DifficultyLevel.Fiendish),
						Tag = DifficultyLevel.Fiendish
					},
					new SegmentedItem
					{
						Content = ResourceDictionary.Get("DifficultyLevel_Nightmare", App.CurrentCulture),
						Foreground = DifficultyLevelConversion.GetForegroundColor(DifficultyLevel.Nightmare),
						Background = DifficultyLevelConversion.GetBackgroundColor(DifficultyLevel.Nightmare),
						Tag = DifficultyLevel.Nightmare
					}
				},
				SelectedIndex = Log2((uint)(int)pref.GetDifficultyLevel(technique)),
				VerticalAlignment = VerticalAlignment.Center
			};
			difficultyLevelControl.SelectionChanged += (_, _) =>
			{
				if (difficultyLevelControl.SelectedItem is SegmentedItem { Tag: DifficultyLevel d })
				{
					pref.AppendOrUpdateValue(technique, d);
				}
			};
			GridLayout.SetRow(difficultyLevelControl, i);
			GridLayout.SetColumn(difficultyLevelControl, 2);

			//
			// Rating
			//
			var ratingControl = new IntegerBox
			{
				Width = 150,
				Value = pref.GetRating(technique),
				Minimum = 0,
				Maximum = 1000000,
				SmallChange = 1,
				LargeChange = 100,
				HorizontalAlignment = HorizontalAlignment.Right,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new(0, 0, 6, 0)
			};
			ratingControl.ValueChanged += (_, _) => pref.AppendOrUpdateValue(technique, ratingControl.Value);
			GridLayout.SetRow(ratingControl, i);
			GridLayout.SetColumn(ratingControl, 3);

			g.Children.Add(nameControl);
			g.Children.Add(englishNameControl);
			g.Children.Add(difficultyLevelControl);
			g.Children.Add(ratingControl);
		}


		static void setRowDefinitions(GridLayout g, TechniqueSet values)
		{
			g.RowDefinitions.Clear();

			// Add a title row
			g.RowDefinitions.Add(r());

			// Add children rows
			for (var i = 0; i < values.Count; i++)
			{
				g.RowDefinitions.Add(r());
			}


			static RowDefinition r() => new() { Height = DefaultHeight };
		}

		static void addTableTitleRow(GridLayout g)
		{
			g.Children.Add(t("TechniqueInfoModifierPage_TechniqueName", 0, HorizontalAlignment.Left));
			g.Children.Add(t("TechniqueInfoModifierPage_TechniqueEnglishName", 1));
			g.Children.Add(t("TechniqueInfoModifierPage_DifficultyLevel", 2));
			g.Children.Add(t("TechniqueInfoModifierPage_DifficultyRating", 3, HorizontalAlignment.Right));


			static TextBlock t(string resourceKey, int column, HorizontalAlignment? horizontalAlignment = null)
			{
				var result = new TextBlock
				{
					Text = ResourceDictionary.Get(resourceKey, App.CurrentCulture),
					HorizontalAlignment = horizontalAlignment ?? HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center,
					FontWeight = FontWeights.Bold
				};
				if (horizontalAlignment is { } alignment)
				{
					Debug.Assert(alignment is HorizontalAlignment.Left or HorizontalAlignment.Right);
					result.Margin = alignment switch
					{
						HorizontalAlignment.Left => new(6, 0, 0, 0),
						HorizontalAlignment.Right => new(0, 0, 6, 0)
					};
				}

				GridLayout.SetRow(result, 0);
				GridLayout.SetColumn(result, column);

				return result;
			}
		}
	}


	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		CurrentTechniqueGroup = TechniqueGroup.Single;
		TechniqueGroupDisplayer.Text = CurrentTechniqueGroup.GetName(App.CurrentCulture);
	}
}
