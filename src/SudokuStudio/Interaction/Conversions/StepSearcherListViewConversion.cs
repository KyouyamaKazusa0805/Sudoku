using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Sudoku.Analytics;
using Sudoku.Analytics.Categorization;
using SudokuStudio.Configuration;
using SudokuStudio.Views.Controls;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about <see cref="StepSearcherListView"/> instances.
/// </summary>
/// <seealso cref="StepSearcherListView"/>
internal static class StepSearcherListViewConversion
{
	public static object? GetStepSearcherSupportedDifficultyLevelCollection(StepSearcherInfo? info)
		=> info is null ? null : GetMatchedStepSearcher(info).DifficultyLevelRange;

	public static object? GetStepSearcherSupportedTechniqueCollection(StepSearcherInfo? info)
		=> info is null ? null : from t in GetMatchedStepSearcher(info).SupportedTechniques orderby t.GetDifficultyLevel() select t;

	public static string GetStepSearcherName(StepSearcherInfo? info) => info is null ? string.Empty : GetMatchedStepSearcher(info).Name;

	public static string GetTechniqueName(Technique technique) => technique.GetName();

	public static Visibility GetDisplayerVisibility(StepSearcherInfo? info) => info is null ? Visibility.Collapsed : Visibility.Visible;

	public static Brush GetTechniqueForeground(Technique technique)
		=> DifficultyLevelConversion.GetForegroundColor(technique.GetDifficultyLevel());

	public static Brush GetTechniqueBackground(Technique technique)
		=> DifficultyLevelConversion.GetBackgroundColor(technique.GetDifficultyLevel());

	private static StepSearcher GetMatchedStepSearcher(StepSearcherInfo info) => StepSearcherPool.GetStepSearchers(info.TypeName, false)[0];
}
