namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a generating strategy items provider.
/// </summary>
public sealed class GeneratingStrategyItemsProvider : IRunningStrategyItemsProvider
{
	/// <inheritdoc/>
	public IList<RunningStrategyItem> Items
		=> [
			new(
				"GeneratingStrategyPage_DifficultyLevel",
				new(
					"GeneratingStrategyPage_DifficultyLevel",
					DiffficultyLevelControlCreator,
					DifficultyLevelInitializedValueDisplayer,
					DifficultyLevelValueRouter
				)
			),
			new(
				"GeneratingStrategyPage_Symmetry",
				new(
					"GeneratingStrategyPage_Symmetry",
					SymmetricTypeControlCreator,
					SymmetricTypeInitializedValueDisplayer,
					SymmetricTypeValueRouter
				)
			),
			new(
				"GeneratingStrategyPage_TechniquesMustIncluded",
				new(
					"GeneratingStrategyPage_TechniquesMustIncluded",
					TechniqueMustIncludedControlCreator,
					TechniqueMustIncludedInitializedValueDisplayer,
					TechniqueMustIncludedValueRouter
				)
			),
			new(
				"GeneratingStrategyPage_IsMinimalPuzzle",
				new(
					"GeneratingStrategyPage_IsMinimalPuzzle",
					IsMinimalControlCreator,
					IsMinimalInitializedValueDisplayer,
					IsMinimalValueRouter
				)
			),
			new(
				"GeneratingStrategyPage_FirstAssignmentAttribute",
				new(
					"GeneratingStrategyPage_FirstAssignmentAttribute",
					FirstAssignmentAttributeControlCreator,
					FirstAssignmentAttributeInitializedValueDisplayer,
					FirstAssignmentAttributeValueRouter
				)
			),
			new(
				"GeneratingStrategyPage_CanRestrictGeneratingGivensCount",
				new(
					"GeneratingStrategyPage_CanRestrictGeneratingGivensCount",
					CanRestrictGeneratingGivensCountControlCreator,
					CanRestrictGeneratingGivensCountInitializedValueDisplayer,
					CanRestrictGeneratingGivensCountValueRouter
				)
			),
			new(
				"GeneratingStrategyPage_GeneratedPuzzleGivensCount",
				new(
					"GeneratingStrategyPage_GeneratedPuzzleGivensCount",
					GeneratedPuzzleGivensCountControlCreator,
					GeneratedPuzzleGivensCountInitializedValueDisplayer,
					GeneratedPuzzleGivensCountValueRouter
				)
			),
			new(
				"GeneratingStrategyPage_IttoryuLength",
				new(
					"GeneratingStrategyPage_IttoryuLength",
					IttoryuLengthControlCreater,
					IttoryuLengthInitializedValueDisplayer,
					IttoryuLengthValueRouter
				)
			)
		];


	private static ComboBox DiffficultyLevelControlCreator()
	{
		var control = new ComboBox
		{
			ItemsSource = (ComboBoxItem[])[
				new() { Content = ResourceDictionary.Get("DifficultyLevel_None", App.CurrentCulture), Tag = DifficultyLevel.Unknown },
				new() { Content = ResourceDictionary.Get("DifficultyLevel_Easy", App.CurrentCulture), Tag = DifficultyLevel.Easy },
				new() { Content = ResourceDictionary.Get("DifficultyLevel_Moderate", App.CurrentCulture), Tag = DifficultyLevel.Moderate },
				new() { Content = ResourceDictionary.Get("DifficultyLevel_Hard", App.CurrentCulture), Tag = DifficultyLevel.Hard },
				new() { Content = ResourceDictionary.Get("DifficultyLevel_Fiendish", App.CurrentCulture), Tag = DifficultyLevel.Fiendish },
				new() { Content = ResourceDictionary.Get("DifficultyLevel_Nightmare", App.CurrentCulture), Tag = DifficultyLevel.Nightmare }
			]
		};
		control.SelectedIndex = Array.IndexOf(
			from item in (ComboBoxItem[])control.ItemsSource select item.Tag,
			((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel
		);

		return control;
	}

	private static ComboBox SymmetricTypeControlCreator()
		=> new()
		{
			ItemsSource = (ComboBoxItem[])[
				new() { Content = ResourceDictionary.Get("SymmetricType_None", App.CurrentCulture), Tag = SymmetricType.None },
				new() { Content = ResourceDictionary.Get("SymmetricType_Central", App.CurrentCulture), Tag = SymmetricType.Central },
				new() { Content = ResourceDictionary.Get("SymmetricType_Diagonal", App.CurrentCulture), Tag = SymmetricType.Diagonal },
				new() { Content = ResourceDictionary.Get("SymmetricType_AntiDiagonal", App.CurrentCulture), Tag = SymmetricType.AntiDiagonal },
				new() { Content = ResourceDictionary.Get("SymmetricType_AntiDiagonal", App.CurrentCulture), Tag = SymmetricType.AntiDiagonal },
				new() { Content = ResourceDictionary.Get("SymmetricType_XAxis", App.CurrentCulture), Tag = SymmetricType.XAxis },
				new() { Content = ResourceDictionary.Get("SymmetricType_YAxis", App.CurrentCulture), Tag = SymmetricType.YAxis },
				new() { Content = ResourceDictionary.Get("SymmetricType_AxisBoth", App.CurrentCulture), Tag = SymmetricType.AxisBoth },
				new() { Content = ResourceDictionary.Get("SymmetricType_DiagonalBoth", App.CurrentCulture), Tag = SymmetricType.DiagonalBoth },
				new() { Content = ResourceDictionary.Get("SymmetricType_All", App.CurrentCulture), Tag = SymmetricType.All }
			],
			SelectedIndex = (int)((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern
		};

	private static Button TechniqueMustIncludedControlCreator()
	{
		var result = new Button { Content = new FontIcon { Glyph = "\uE8A7" } };
		result.Click += (_, _) => ((App)Application.Current).WindowManager
			.ActiveWindows
			.OfType<MainWindow>()
			.First()
			.NavigateToPage(typeof(TechniqueSelectionPage));
		return result;
	}

	private static ToggleSwitch IsMinimalControlCreator()
		=> new() { IsOn = ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal };

	private static ToggleSwitch FirstAssignmentAttributeControlCreator()
		=> new() { IsOn = ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl };

	private static ToggleSwitch CanRestrictGeneratingGivensCountControlCreator()
		=> new() { IsOn = ((App)Application.Current).Preference.UIPreferences.CanRestrictGeneratingGivensCount };

	private static IntegerBox GeneratedPuzzleGivensCountControlCreator()
		=> new()
		{
			Minimum = 17,
			Maximum = 80,
			SmallChange = 1,
			LargeChange = 4,
			Value = ((App)Application.Current).Preference.UIPreferences is var uiPref && uiPref.CanRestrictGeneratingGivensCount ? uiPref.GeneratedPuzzleGivensCount : 0
		};

	private static IntegerBox IttoryuLengthControlCreater()
		=> new()
		{
			Minimum = 0,
			Maximum = 9,
			SmallChange = 1,
			LargeChange = 3,
			Value = ((App)Application.Current).Preference.UIPreferences is var uiPref && uiPref.GeneratorDifficultyLevel == DifficultyLevel.Easy ? uiPref.IttoryuLength : 0
		};

	private static string DifficultyLevelInitializedValueDisplayer()
		=> DifficultyLevelConversion.GetNameWithDefault(
			((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel,
			ResourceDictionary.Get("DifficultyLevel_None", App.CurrentCulture)
		);

	private static string SymmetricTypeInitializedValueDisplayer()
		=> ResourceDictionary.Get($"SymmetricType_{((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern}");

	private static string TechniqueMustIncludedInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.GeneratorSelectedTechniques switch
		{
			[] => ResourceDictionary.Get("TechniqueSelector_NoTechniqueSelected", App.CurrentCulture),
			var n => string.Join(", ", [.. from s in n select s.GetName(App.CurrentCulture)])
		};

	private static string IsMinimalInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal.ToString();

	private static string FirstAssignmentAttributeInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl switch
		{
			true => ResourceDictionary.Get("GeneratingStrategyPage_PearlPuzzle", App.CurrentCulture),
			false => ResourceDictionary.Get("GeneratingStrategyPage_NormalPuzzle", App.CurrentCulture),
			//_ => ResourceDictionary.Get("GeneratingStrategyPage_DiamondPuzzle", App.CurrentCulture)
		};

	private static string CanRestrictGeneratingGivensCountInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.CanRestrictGeneratingGivensCount ? ResourceDictionary.Get("Yes", App.CurrentCulture) : ResourceDictionary.Get("No", App.CurrentCulture);

	private static string GeneratedPuzzleGivensCountInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.CanRestrictGeneratingGivensCount
			? string.Format(
				ResourceDictionary.Get("GeneratingStrategyPage_GivensHave", App.CurrentCulture),
				((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleGivensCount
			)
			: "/";

	private static string IttoryuLengthInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.IttoryuLength switch
		{
			-1 => "/",
			0 => ResourceDictionary.Get("GeneratingStrategyPage_ZeroIttoryu", App.CurrentCulture),
			9 => ResourceDictionary.Get("GeneratingStrategyPage_RealIttoryu", App.CurrentCulture),
			var i and > 0 and < 9 => i.ToString(),
			_ => ResourceDictionary.Get("GeneratingStrategyPage_Error", App.CurrentCulture)
		};

	private static void DifficultyLevelValueRouter(FrameworkElement c, TextBlock _)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel = (DifficultyLevel)((ComboBoxItem)((ComboBox)c).SelectedItem).Tag!;

	private static void SymmetricTypeValueRouter(FrameworkElement c, TextBlock _)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern = (SymmetricType)((ComboBoxItem)((ComboBox)c).SelectedItem).Tag!;

	private static void TechniqueMustIncludedValueRouter(FrameworkElement c, TextBlock t)
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		var expectedDifficultyLevel = uiPref.GeneratorSelectedTechniques
			? uiPref.GeneratorSelectedTechniques.DifficultyRange.GetAllFlags()[^1]
			: DifficultyLevel.Unknown;
		var condition = uiPref.GeneratorDifficultyLevel is var gdl && gdl < expectedDifficultyLevel && gdl != DifficultyLevel.Unknown;
		t.Text = condition
			? string.Format(
				ResourceDictionary.Get("GeneratingStrategyPage_DifficultyLevelMustBeGreaterThan", App.CurrentCulture),
				DifficultyLevelConversion.GetName(expectedDifficultyLevel)
			)
			: string.Empty;
		t.Foreground = condition ? new SolidColorBrush(Colors.Red) : null;
	}

	private static void IsMinimalValueRouter(FrameworkElement c, TextBlock _)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal = ((ToggleSwitch)c).IsOn;

	private static void FirstAssignmentAttributeValueRouter(FrameworkElement c, TextBlock _)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl = ((ToggleSwitch)c).IsOn;

	private static void CanRestrictGeneratingGivensCountValueRouter(FrameworkElement c, TextBlock _)
		=> ((App)Application.Current).Preference.UIPreferences.CanRestrictGeneratingGivensCount = ((ToggleSwitch)c).IsOn;

	private static void GeneratedPuzzleGivensCountValueRouter(FrameworkElement c, TextBlock t)
	{
		var condition = ((App)Application.Current).Preference.UIPreferences.CanRestrictGeneratingGivensCount;
		((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleGivensCount = condition ? ((IntegerBox)c).Value : -1;

		t.Text = condition ? string.Empty : ResourceDictionary.Get("GeneratingStrategyPage_GivensEnabledMustBePreviousControlSetTrue", App.CurrentCulture);
		t.Foreground = condition ? null : new SolidColorBrush(Colors.Orange);
	}

	private static void IttoryuLengthValueRouter(FrameworkElement c, TextBlock t)
	{
		var condition = ((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel == DifficultyLevel.Easy;
		((App)Application.Current).Preference.UIPreferences.IttoryuLength = condition ? ((IntegerBox)c).Value : -1;

		t.Text = condition ? string.Empty : ResourceDictionary.Get("GeneratingStrategyPage_IttroyuLengthEnabledMustBeEasy", App.CurrentCulture);
		t.Foreground = condition ? null : new SolidColorBrush(Colors.Orange);
	}
}
