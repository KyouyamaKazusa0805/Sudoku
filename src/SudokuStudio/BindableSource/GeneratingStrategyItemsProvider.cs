using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Sudoku.Analytics.Categorization;
using Sudoku.Concepts;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;
using SudokuStudio.Interaction.Conversions;
using SudokuStudio.Views.Controls;
using static SudokuStudio.Strings.StringsAccessor;

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
	{
		var previousSelectedTechnique = ((App)Application.Current).Preference.UIPreferences.SelectedTechnique;
		var result = new TechniqueSelector();
		var foundIndex = Array.FindIndex(result.ItemsSource, element => element.Technique == previousSelectedTechnique);
		result.SelectedIndex = foundIndex == -1 ? 0 : foundIndex;
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
			GetString("DifficultyLevel_None")
		);

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

	private static string CanRestrictGeneratingGivensCountInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.CanRestrictGeneratingGivensCount ? GetString("Yes") : GetString("No");

	private static string GeneratedPuzzleGivensCountInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.CanRestrictGeneratingGivensCount
			? string.Format(
				GetString("GeneratingStrategyPage_GivensHave"),
				((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleGivensCount
			)
			: "/";

	private static string IttoryuLengthInitializedValueDisplayer()
		=> ((App)Application.Current).Preference.UIPreferences.IttoryuLength switch
		{
			-1 => "/",
			0 => GetString("GeneratingStrategyPage_ZeroIttoryu"),
			9 => GetString("GeneratingStrategyPage_RealIttoryu"),
			var i and > 0 and < 9 => i.ToString(),
			_ => GetString("GeneratingStrategyPage_Error")
		};

	private static void DifficultyLevelValueRouter(FrameworkElement c, TextBlock _)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel = (DifficultyLevel)((ComboBoxItem)((ComboBox)c).SelectedItem).Tag!;

	private static void SymmetricTypeValueRouter(FrameworkElement c, TextBlock _)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern = (SymmetricType)((ComboBoxItem)((ComboBox)c).SelectedItem).Tag!;

	private static void TechniqueMustIncludedValueRouter(FrameworkElement c, TextBlock t)
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		uiPref.SelectedTechnique = c switch
		{
			TechniqueSelector { ItemsSource: var source, SelectedIndex: var index } => index switch
			{
				-1 => Technique.None,
				_ => source[index].Technique
			},
			_ => throw new InvalidOperationException("The status is invalid.")
		};

		var expectedDifficultyLevel = uiPref.SelectedTechnique.GetDifficultyLevel();
		var condition = uiPref.GeneratorDifficultyLevel is var gdl && gdl < expectedDifficultyLevel && gdl != DifficultyLevel.Unknown;
		t.Text = condition
			? string.Format(
				GetString("GeneratingStrategyPage_DifficultyLevelMustBeGreaterThan"),
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

		t.Text = condition ? string.Empty : GetString("GeneratingStrategyPage_GivensEnabledMustBePreviousControlSetTrue");
		t.Foreground = condition ? null : new SolidColorBrush(Colors.Orange);
	}

	private static void IttoryuLengthValueRouter(FrameworkElement c, TextBlock t)
	{
		var condition = ((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel == DifficultyLevel.Easy;
		((App)Application.Current).Preference.UIPreferences.IttoryuLength = condition ? ((IntegerBox)c).Value : -1;

		t.Text = condition ? string.Empty : GetString("GeneratingStrategyPage_IttroyuLengthEnabledMustBeEasy");
		t.Foreground = condition ? null : new SolidColorBrush(Colors.Orange);
	}
}
