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
			)
		];


	private static ComboBox DiffficultyLevelControlCreator()
	{
		var control = new ComboBox
		{
			ItemsSource = (ComboBoxItem[])[
				new() { Content = GetString("DifficultyLevel_None"), Tag = DifficultyLevel.Unknown },
				new() { Content = GetString("DifficultyLevel_Easy"), Tag = DifficultyLevel.Easy },
				new() { Content = GetString("DifficultyLevel_Moderate"), Tag = DifficultyLevel.Moderate },
				new() { Content = GetString("DifficultyLevel_Hard"), Tag = DifficultyLevel.Hard },
				new() { Content = GetString("DifficultyLevel_Fiendish"), Tag = DifficultyLevel.Fiendish },
				new() { Content = GetString("DifficultyLevel_Nightmare"), Tag = DifficultyLevel.Nightmare }
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
				new() { Content = GetString("SymmetricType_None"), Tag = SymmetricType.None },
				new() { Content = GetString("SymmetricType_Central"), Tag = SymmetricType.Central },
				new() { Content = GetString("SymmetricType_Diagonal"), Tag = SymmetricType.Diagonal },
				new() { Content = GetString("SymmetricType_AntiDiagonal"), Tag = SymmetricType.AntiDiagonal },
				new() { Content = GetString("SymmetricType_AntiDiagonal"), Tag = SymmetricType.AntiDiagonal },
				new() { Content = GetString("SymmetricType_XAxis"), Tag = SymmetricType.XAxis },
				new() { Content = GetString("SymmetricType_YAxis"), Tag = SymmetricType.YAxis },
				new() { Content = GetString("SymmetricType_AxisBoth"), Tag = SymmetricType.AxisBoth },
				new() { Content = GetString("SymmetricType_DiagonalBoth"), Tag = SymmetricType.DiagonalBoth },
				new() { Content = GetString("SymmetricType_All"), Tag = SymmetricType.All }
			],
			SelectedIndex = (int)((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern
		};

	private static TechniqueSelector TechniqueMustIncludedControlCreator()
		=> new() { SelectedIndex = (int)((App)Application.Current).Preference.UIPreferences.SelectedTechnique };

	private static ToggleSwitch IsMinimalControlCreator()
		=> new() { IsOn = ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal };

	private static ToggleSwitch FirstAssignmentAttributeControlCreator()
		=> new() { IsOn = ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl };

	private static string DifficultyLevelInitializedValueDisplayer()
		=> DifficultyLevelConversion.GetName(((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel);

	private static string SymmetricTypeInitializedValueDisplayer()
		=> GetString($"SymmetricType_{((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern}");

	private static string TechniqueMustIncludedInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.SelectedTechnique switch
		{
			Technique.None => GetString("TechniqueSelector_NoTechniqueSelected"),
			var n => $"{n.GetName()} ({n.GetEnglishName()})"
		};

	private static string IsMinimalInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal.ToString();

	private static string FirstAssignmentAttributeInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl switch
		{
			true => GetString("GeneratingStrategyPage_PearlPuzzle"),
			false => GetString("GeneratingStrategyPage_NormalPuzzle"),
			//_ => GetString("GeneratingStrategyPage_DiamondPuzzle")
		};

	private static void DifficultyLevelValueRouter(FrameworkElement c)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel = (DifficultyLevel)((ComboBoxItem)((ComboBox)c).SelectedItem).Tag!;

	private static void SymmetricTypeValueRouter(FrameworkElement c)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern = (SymmetricType)((ComboBoxItem)((ComboBox)c).SelectedItem).Tag!;

	private static void TechniqueMustIncludedValueRouter(FrameworkElement c)
		=> ((App)Application.Current).Preference.UIPreferences.SelectedTechnique = c switch
		{
			TechniqueSelector { ItemsSource: var source, SelectedIndex: var index } => index switch { -1 => Technique.None, _ => source[index].Technique },
			_ => throw new InvalidOperationException("The status is invalid.")
		};

	private static void IsMinimalValueRouter(FrameworkElement c)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal = ((ToggleSwitch)c).IsOn;

	private static void FirstAssignmentAttributeValueRouter(FrameworkElement c)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl = ((ToggleSwitch)c).IsOn;
}
