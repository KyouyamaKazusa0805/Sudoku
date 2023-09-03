namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a generating strategy items provider.
/// </summary>
public sealed class GeneratingStrategyItemsProvider : IRunningStrategyItemsProvider
{
	private const string GeneratingStrategyPage_DifficultyLevel = nameof(GeneratingStrategyPage_DifficultyLevel);

	private const string GeneratingStrategyPage_Symmetry = nameof(GeneratingStrategyPage_Symmetry);

	private const string GeneratingStrategyPage_TechniquesMustIncluded = nameof(GeneratingStrategyPage_TechniquesMustIncluded);

	private const string GeneratingStrategyPage_IsMinimalPuzzle = nameof(GeneratingStrategyPage_IsMinimalPuzzle);

	private const string GeneratingStrategyPage_FirstAssignmentAttribute = nameof(GeneratingStrategyPage_FirstAssignmentAttribute);


	/// <inheritdoc/>
	public IList<RunningStrategyItem> Items
		=> [
			new(
				GeneratingStrategyPage_DifficultyLevel,
				new(
					GeneratingStrategyPage_DifficultyLevel,
					static () => new ComboBox
					{
						SelectedIndex = 0,
						ItemsSource = (ComboBoxItem[])[
							new() { Content = GetString("DifficultyLevel_None"), Tag = DifficultyLevel.Unknown },
							new() { Content = GetString("DifficultyLevel_Easy"), Tag = DifficultyLevel.Easy },
							new() { Content = GetString("DifficultyLevel_Moderate"), Tag = DifficultyLevel.Moderate },
							new() { Content = GetString("DifficultyLevel_Hard"), Tag = DifficultyLevel.Hard },
							new() { Content = GetString("DifficultyLevel_Fiendish"), Tag = DifficultyLevel.Fiendish },
							new() { Content = GetString("DifficultyLevel_Nightmare"), Tag = DifficultyLevel.Nightmare }
						]
					},
					static () => DifficultyLevelConversion.GetName(((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel),
					static c => ((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel = (DifficultyLevel)((ComboBoxItem)((ComboBox)c).SelectedItem).Tag!
				)
			),
			new(
				GeneratingStrategyPage_Symmetry,
				new(
					GeneratingStrategyPage_Symmetry,
					static () => new ComboBox
					{
						SelectedIndex = 0,
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
						]
					},
					static () => GetString($"SymmetricType_{((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern}"),
					static c => ((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern = (SymmetricType)((ComboBoxItem)((ComboBox)c).SelectedItem).Tag!
				)
			),
			new(
				GeneratingStrategyPage_TechniquesMustIncluded,
				new(
					GeneratingStrategyPage_TechniquesMustIncluded,
					static () => new TechniqueSelector { SelectedIndex = 0 },
					static () => ((App)Application.Current).Preference.UIPreferences.SelectedTechnique switch
					{
						Technique.None => GetString("TechniqueSelector_NoTechniqueSelected"),
						var n => $"{n.GetName()} ({n.GetEnglishName()})"
					},
					static c => ((App)Application.Current).Preference.UIPreferences.SelectedTechnique = c switch
					{
						TechniqueSelector { ItemsSource: var source, SelectedIndex: var index }
							=> index switch { -1 => Technique.None, _ => source[index].Technique },
						_
							=> throw new InvalidOperationException("The status is invalid.")
					}
				)
			),
			new(
				GeneratingStrategyPage_IsMinimalPuzzle,
				new(
					GeneratingStrategyPage_IsMinimalPuzzle,
					static () => new ToggleSwitch(),
					static () => ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal.ToString(),
					static c => ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal = ((ToggleSwitch)c).IsOn
				)
			),
			new(
				GeneratingStrategyPage_FirstAssignmentAttribute,
				new(
					GeneratingStrategyPage_FirstAssignmentAttribute,
					static () => new ToggleSwitch(),
					static () => ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl switch
					{
						true => GetString("GeneratingStrategyPage_PearlPuzzle"),
						false => GetString("GeneratingStrategyPage_NormalPuzzle"),
						//_ => GetString("GeneratingStrategyPage_DiamondPuzzle")
					},
					static c => ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl = ((ToggleSwitch)c).IsOn
				)
			)
		];
}
