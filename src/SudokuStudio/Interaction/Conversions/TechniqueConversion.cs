using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Sudoku.Analytics.Categorization;
using Sudoku.Compatibility.Hodoku;
using Sudoku.Compatibility.SudokuExplainer;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about technique.
/// </summary>
internal static class TechniqueConversion
{
	public static int GetDisplayNameColumnSpan(TechniqueFeature feature) => feature == TechniqueFeature.None ? 2 : 1;

	public static string GetName(Technique technique) => technique == Technique.None ? string.Empty : technique.GetName(CurrentCultureInfo);

	public static string GetEnglishName(Technique technique)
		=> technique == Technique.None ? string.Empty : technique.GetEnglishName() ?? GetString("TechniqueSelectionPage_NoEnglishName");

	public static string GetDifficultyLevel(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: DifficultyLevelConversion.GetNameWithDefault(technique.GetDifficultyLevel(), GetString("TechniqueSelectionPage_NoDifficultyLevel"));

	public static string GetAliasNames(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: technique.GetAliases() is { Length: not 0 } a ? string.Join(", ", a) : GetString("TechniqueSelectionPage_NoAliases");

	public static string GetAbbreviation(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: technique.GetAbbreviation() ?? GetString("TechniqueSelectionPage_NoAbbreviation");

	public static string GetGroup(Technique technique)
		=> technique == Technique.None ? string.Empty : technique.GetGroup().GetName(CurrentCultureInfo);

	public static string GetGroupShortenedName(Technique technique)
		=> technique == Technique.None ? string.Empty : technique.GetGroup().GetShortenedName(CurrentCultureInfo);

	public static string GetFeature(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: GetStringResourceViaFeature(technique.GetFeature()) is var p and not "" ? p : GetString("TechniqueSelectionPage_NoExtraFeatures");

	public static string GetFeatureDescription(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: GetStringTooltipViaFeature(technique.GetFeature()) is { Length: not 0 } p ? p : GetString("TechniqueSelectionPage_NoExtraFeaturesDescription");

	public static string GetSudokuExplainerDifficultyRange(Technique technique)
	{
		var advancedText = GetString("TechniqueSelectionPage_AdvancedDefined");
		var nullDefinedText = GetString("TechniqueSelectionPage_NullDefined");
		return technique switch
		{
			Technique.None => string.Empty,
			_ => SudokuExplainerCompatibility.GetDifficultyRatingRange(technique) switch
			{
				({ IsRange: true, Min: var d1, Max: var d2 }, { IsRange: true, Min: var d3, Max: var d4 })
					=> $"{d1:#.0} - {d2:#.0}{Environment.NewLine}{d3:#.0} - {d4:#.0}{advancedText}",
				({ IsRange: true, Min: var d1, Max: var d2 }, { IsRange: false, Min: var d3 })
					=> $"{d1:#.0} - {d2:#.0}{Environment.NewLine}{d3:#.0}{advancedText}",
				({ IsRange: false, Min: var d1 }, { IsRange: true, Min: var d3, Max: var d4 })
					=> $"{d1:#.0}{Environment.NewLine}{d3:#.0} - {d4:#.0}{advancedText}",
				({ Min: var d1 }, { Min: var d3 })
					=> $"{d1:#.0}{Environment.NewLine}{d3:#.0}{advancedText}",
				({ IsRange: false, Min: var d }, null)
					=> $"{d:#.0}",
				({ IsRange: true, Min: var d1, Max: var d2 }, null)
					=> $"{d1:#.0} - {d2:#.0}",
				(_, { IsRange: false, Min: var d })
					=> $"{d:#.0}{advancedText}",
				(_, { IsRange: true, Min: var d1, Max: var d2 })
					=> $"{d1:#.0} - {d2:#.0}{advancedText}",
				_
					=> nullDefinedText,
			}
		};
	}

	public static string GetHodokuDifficultyRating(Technique technique)
		=> technique == Technique.None ? string.Empty : HodokuCompatibility.GetDifficultyRating(technique, out var difficultyLevel) switch
		{
			{ } value => $"{value}{Token("OpenBrace")}{GetString(difficultyLevel switch
			{
				HodokuDifficultyLevel.Easy => "HodokuDifficultyLevel_Easy",
				HodokuDifficultyLevel.Medium => "HodokuDifficultyLevel_Medium",
				HodokuDifficultyLevel.Hard => "HodokuDifficultyLevel_Hard",
				HodokuDifficultyLevel.Unfair => "HodokuDifficultyLevel_Unfair",
				HodokuDifficultyLevel.Extreme => "HodokuDifficultyLevel_Extreme"
			})}{Token("ClosedBrace")}",
			_ => GetString("TechniqueSelectionPage_NullDefined")
		};

	public static string GetHodokuPrefix(Technique technique)
		=> technique == Technique.None ? string.Empty : HodokuCompatibility.GetHodokuPrefix(technique) ?? GetString("TechniqueSelectionPage_NoHodokuPrefix");

	public static string GetStringResourceViaFeature(TechniqueFeature feature)
		=> feature switch
		{
			TechniqueFeature.HardToBeGenerated => GetString("TechniqueFeature_HardToBeGeneratedShort"),
			TechniqueFeature.WillBeReplacedByOtherTechnique => GetString("TechniqueFeature_WillBeReplacedByOtherTechniqueShort"),
			TechniqueFeature.OnlyExistInTheory => GetString("TechniqueFeature_OnlyExistInTheoryShort"),
			TechniqueFeature.NotImplemented => GetString("TechniqueFeature_NotImplementedShort"),
			TechniqueFeature.DirectTechniques => GetString("TechniqueFeature_DirectTechniquesShort"),
			_ => string.Empty
		};

	public static string? GetStringTooltipViaFeature(TechniqueFeature feature)
		=> feature switch
		{
			TechniqueFeature.HardToBeGenerated => GetString("TechniqueFeature_HardToBeGenerated"),
			TechniqueFeature.WillBeReplacedByOtherTechnique => GetString("TechniqueFeature_WillBeReplacedByOtherTechnique"),
			TechniqueFeature.OnlyExistInTheory => GetString("TechniqueFeature_OnlyExistInTheory"),
			TechniqueFeature.NotImplemented => GetString("TechniqueFeature_NotImplemented"),
			TechniqueFeature.DirectTechniques => GetString("TechniqueFeature_DirectTechniques"),
			_ => null
		};

	public static Visibility GetExtraDescriptionVisibility(TechniqueFeature feature)
		=> feature == TechniqueFeature.None ? Visibility.Collapsed : Visibility.Visible;

	public static Visibility GetFeatureVisibility(Technique technique) => GetExtraDescriptionVisibility(technique.GetFeature());

	public static Brush GetBrush(TechniqueFeature feature)
		=> new SolidColorBrush(feature == TechniqueFeature.None ? Colors.Black : Colors.LightGray);

	public static string[] GetIntroductionLinks(Technique technique) => technique.GetIntroductionHyperlinks();
}
