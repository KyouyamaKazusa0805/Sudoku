#define HORIZONTAL_GRID_LINES
#undef VERTICAL_GRID_LINES
#if !HORIZONTAL_GRID_LINES && VERTICAL_GRID_LINES && !DEBUG
#warning You cannot configure for only vertical grid lines because it is really ugly in UI. Either configure 'DEBUG' symbol or break "vertical grid line only" rule and continue.
#endif

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a technique information modifier page.
/// </summary>
[DependencyProperty<int>("CurrentIndex", DefaultValue = -1, Accessibility = Accessibility.Internal)]
public sealed partial class TechniqueInfoModifierPage : Page
{
	/// <summary>
	/// Indicates the maximum rating value can be set.
	/// </summary>
	private const int MaximumRatingValue = 1000000;

	/// <summary>
	/// Indicates the default with for rating control.
	/// </summary>
	private const int DefaultWidthForRatingControl = 190;


	/// <summary>
	/// Indicates the default grid row height.
	/// </summary>
	private static readonly GridLength DefaultHeight = new(50, GridUnitType.Pixel);

	/// <summary>
	/// Indicates the margin value that only inserts for left.
	/// </summary>
	private static readonly Thickness LeftMargin = new(6, 0, 0, 0);

	/// <summary>
	/// Indicates the margin value that only inserts for right.
	/// </summary>
	private static readonly Thickness RightMargin = new(0, 0, 6, 0);


	/// <summary>
	/// Initializes a <see cref="TechniqueInfoModifierPage"/> instance.
	/// </summary>
	public TechniqueInfoModifierPage() => InitializeComponent();


	[Callback]
	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	private static async void CurrentIndexPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (TechniqueInfoModifierPage p, { NewValue: int techniqueGroupIndex, OldValue: int originalTechniqueGroupIndex }))
		{
			return;
		}

		var techniqueGroup = TechniqueConversion.ConfigurableTechniqueGroups[techniqueGroupIndex];

		// Change text block.
		p.TechniqueGroupDisplayer.Text = techniqueGroup.GetName(App.CurrentCulture);

		// Change values.
		var values = techniqueGroup.GetTechniques(static technique => technique.SupportsCustomizingDifficulty());
		var g = p.MainGrid;

		clearChildren(g);
		setRowDefinitions(g, values);
		addTableTitleRow(g);

		// Add for children controls.
		var pref = ((App)Application.Current).Preference.TechniqueInfoPreferences;
		for (var (rowIndex, i) = (1, 0); i < values.Count; rowIndex++, i++)
		{
			addRowDefinition(g);

			var technique = values[i];
			var name = technique.GetName(App.CurrentCulture);
			var englishName = technique.GetEnglishName();

			//
			// Name
			//
			var nameControl = new TextBlock
			{
				Text = name,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = LeftMargin
			};
			GridLayout.SetRow(nameControl, rowIndex);
			GridLayout.SetColumn(nameControl, 0);

			//
			// English name
			//
			var englishNameControl = new TextBlock
			{
				Text = englishName,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = LeftMargin
			};
			GridLayout.SetRow(englishNameControl, rowIndex);
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
				SelectedIndex = Log2((uint)(int)pref.GetDifficultyLevelOrDefault(technique)),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			difficultyLevelControl.SelectionChanged += (_, _) =>
			{
				if (difficultyLevelControl.SelectedItem is SegmentedItem { Tag: DifficultyLevel d })
				{
					pref.AppendOrUpdateDifficultyLevel(technique, d);
				}
			};
			GridLayout.SetRow(difficultyLevelControl, rowIndex);
			GridLayout.SetColumn(difficultyLevelControl, 2);

			//
			// Rating
			//
			var supportedModes = technique.GetSupportedPencilmarkVisibilityModes();
			var ratingControl = default(IntegerBox);
			var hasIndirectRating = supportedModes.HasFlag(PencilmarkVisibility.Indirect);
			if (hasIndirectRating)
			{
				ratingControl = new IntegerBox
				{
					Width = DefaultWidthForRatingControl,
					Value = pref.GetRatingOrDefault(technique),
					Minimum = 0,
					Maximum = MaximumRatingValue,
					SmallChange = 1,
					LargeChange = 100,
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center,
					Margin = RightMargin
				};
				ratingControl.ValueChanged += (_, _) => pref.AppendOrUpdateRating(technique, ratingControl.Value);
				GridLayout.SetRow(ratingControl, rowIndex);
				GridLayout.SetColumn(ratingControl, 3);
			}

			//
			// Direct rating
			//
			var directRatingControl = default(IntegerBox);
			var hasDirectRating = supportedModes.HasFlag(PencilmarkVisibility.Direct);
			if (hasDirectRating)
			{
				directRatingControl = new()
				{
					Width = DefaultWidthForRatingControl,
					Value = pref.GetRatingOrDefault(technique),
					Minimum = 0,
					Maximum = MaximumRatingValue,
					SmallChange = 1,
					LargeChange = 100,
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center,
					Margin = RightMargin
				};
				directRatingControl.ValueChanged += (_, _) => pref.AppendOrUpdateDirectRating(technique, directRatingControl.Value);
				GridLayout.SetRow(directRatingControl, rowIndex);
				GridLayout.SetColumn(directRatingControl, 4);
			}

#if HORIZONTAL_GRID_LINES
			//
			// Horizontal grid line
			//
			var borderGridLine = new Border { BorderBrush = new SolidColorBrush(Colors.Gray), BorderThickness = new(0, .5, 0, 0) };
			Canvas.SetZIndex(borderGridLine, -1);
			GridLayout.SetRow(borderGridLine, rowIndex);
			GridLayout.SetColumn(borderGridLine, 0);
			GridLayout.SetColumnSpan(borderGridLine, 5);
#endif

			await Task.Run(
				() => p.DispatcherQueue.TryEnqueue(
					() =>
					{
						g.Children.Add(nameControl);
						g.Children.Add(englishNameControl);
						g.Children.Add(difficultyLevelControl);
						if (hasIndirectRating)
						{
							Debug.Assert(ratingControl is not null);
							g.Children.Add(ratingControl);
						}
						if (hasDirectRating)
						{
							Debug.Assert(directRatingControl is not null);
							g.Children.Add(directRatingControl);
						}

#if HORIZONTAL_GRID_LINES
						g.Children.Add(borderGridLine);
#endif
					}
				)
			);
			await Task.Delay(10);
		}

#if VERTICAL_GRID_LINES
		//
		// Vertical border lines
		//
		for (var (i, cr, cc) = (1, g.RowDefinitions.Count, g.ColumnDefinitions.Count); i <= cc - 1; i++)
		{
			var borderGridLine = new Border { BorderBrush = new SolidColorBrush(Colors.Gray), BorderThickness = new(0, 0, .5, 0) };
			Canvas.SetZIndex(borderGridLine, -1);
			GridLayout.SetRow(borderGridLine, 0);
			GridLayout.SetRowSpan(borderGridLine, cr);
			GridLayout.SetColumn(borderGridLine, i - 1);

			g.Children.Add(borderGridLine);
		}
#endif

		p.MovePreviousButton.Visibility = Visibility.Visible;
		p.MoveNextButton.Visibility = Visibility.Visible;


		static void clearChildren(GridLayout g) => g.Children.Clear();

		static void setRowDefinitions(GridLayout g, TechniqueSet values)
		{
			g.RowDefinitions.Clear();

			// This is a title row.
			addRowDefinition(g);
		}

		static void addRowDefinition(GridLayout g) => g.RowDefinitions.Add(r());

		static void addTableTitleRow(GridLayout g)
		{
			g.Children.Add(t("TechniqueInfoModifierPage_TechniqueName", 0, HorizontalAlignment.Center));
			g.Children.Add(t("TechniqueInfoModifierPage_TechniqueEnglishName", 1, HorizontalAlignment.Center));
			g.Children.Add(t("TechniqueInfoModifierPage_DifficultyLevel", 2, HorizontalAlignment.Center));
			g.Children.Add(t("TechniqueInfoModifierPage_DifficultyRating", 3, HorizontalAlignment.Center));
			g.Children.Add(t("TechniqueInfoModifierPage_DifficultyDirectRating", 4, HorizontalAlignment.Center));
		}

		static RowDefinition r() => new() { Height = DefaultHeight };

		static TextBlock t(string resourceKey, int column, HorizontalAlignment? horizontalAlignment = null)
		{
			var result = new TextBlock
			{
				Text = ResourceDictionary.Get(resourceKey, App.CurrentCulture),
				HorizontalAlignment = horizontalAlignment ?? HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				FontWeight = FontWeights.Bold,
				FontSize = 18,
				Margin = horizontalAlignment switch
				{
					HorizontalAlignment.Left => LeftMargin,
					HorizontalAlignment.Right => RightMargin,
					_ => new(0)
				}
			};
			GridLayout.SetRow(result, 0);
			GridLayout.SetColumn(result, column);

			return result;
		}
	}


	private void MovePreviousButton_Click(object sender, RoutedEventArgs e)
	{
		MovePreviousButton.Visibility = Visibility.Collapsed;
		MoveNextButton.Visibility = Visibility.Collapsed;
		CurrentIndex--;
	}

	private void MoveNextButton_Click(object sender, RoutedEventArgs e)
	{
		MovePreviousButton.Visibility = Visibility.Collapsed;
		MoveNextButton.Visibility = Visibility.Collapsed;
		CurrentIndex++;
	}

	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		MovePreviousButton.Visibility = Visibility.Collapsed;
		MoveNextButton.Visibility = Visibility.Collapsed;
		CurrentIndex = 0;
	}

	private void ScaleValueBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
		=> ((App)Application.Current).Preference.TechniqueInfoPreferences.RatingScale = (decimal)sender.Value;
}
