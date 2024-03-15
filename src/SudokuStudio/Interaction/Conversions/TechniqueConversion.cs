namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about technique.
/// </summary>
internal static class TechniqueConversion
{
	public static int GetDisplayNameColumnSpan(TechniqueFeature feature) => feature == TechniqueFeature.None ? 2 : 1;

	public static string GetName(Technique technique) => technique == Technique.None ? string.Empty : technique.GetName(App.CurrentCulture);

	public static string GetEnglishName(Technique technique) => technique == Technique.None ? string.Empty : technique.GetEnglishName();

	public static string GetDifficultyLevel(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: DifficultyLevelConversion.GetNameWithDefault(technique.GetDifficultyLevel(), ResourceDictionary.Get("TechniqueSelectionPage_NoDifficultyLevel", App.CurrentCulture));

	public static string GetAliasNames(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: technique.GetAliasedNames(App.CurrentCulture) is { Length: not 0 } a ? string.Join(", ", a) : ResourceDictionary.Get("TechniqueSelectionPage_NoAliases", App.CurrentCulture);

	public static string GetAbbreviation(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: technique.GetAbbreviation() ?? ResourceDictionary.Get("TechniqueSelectionPage_NoAbbreviation", App.CurrentCulture);

	public static string GetGroup(Technique technique)
		=> technique == Technique.None ? string.Empty : technique.GetGroup().GetName(App.CurrentCulture);

	public static string GetGroupShortenedName(Technique technique)
		=> technique == Technique.None ? string.Empty : technique.GetGroup().GetShortenedName(App.CurrentCulture);

	public static string GetFeature(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: GetStringResourceViaFeature(technique.GetFeature()) is var p and not "" ? p : ResourceDictionary.Get("TechniqueSelectionPage_NoExtraFeatures", App.CurrentCulture);

	public static string GetFeatureDescription(Technique technique)
		=> technique == Technique.None
			? string.Empty
			: GetStringTooltipViaFeature(technique.GetFeature()) is { Length: not 0 } p ? p : ResourceDictionary.Get("TechniqueSelectionPage_NoExtraFeaturesDescription", App.CurrentCulture);

	public static string GetSudokuExplainerDifficultyRange(Technique technique)
	{
		var advancedText = ResourceDictionary.Get("TechniqueSelectionPage_AdvancedDefined", App.CurrentCulture);
		var nullDefinedText = ResourceDictionary.Get("TechniqueSelectionPage_NullDefined", App.CurrentCulture);
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
		=> technique == Technique.None ? string.Empty : HodokuCompatibility.GetDifficultyScore(technique, out var difficultyLevel) switch
		{
			{ } value => $"{value}{ResourceDictionary.Get("_Token_OpenBrace", App.CurrentCulture)}{ResourceDictionary.Get(difficultyLevel switch
			{
				HodokuDifficultyLevel.Easy => "HodokuDifficultyLevel_Easy",
				HodokuDifficultyLevel.Medium => "HodokuDifficultyLevel_Medium",
				HodokuDifficultyLevel.Hard => "HodokuDifficultyLevel_Hard",
				HodokuDifficultyLevel.Unfair => "HodokuDifficultyLevel_Unfair",
				HodokuDifficultyLevel.Extreme => "HodokuDifficultyLevel_Extreme"
			}, App.CurrentCulture)}{ResourceDictionary.Get("_Token_ClosedBrace", App.CurrentCulture)}",
			_ => ResourceDictionary.Get("TechniqueSelectionPage_NullDefined", App.CurrentCulture)
		};

	public static string GetHodokuPrefix(Technique technique)
		=> technique == Technique.None ? string.Empty : HodokuCompatibility.GetHodokuLibraryPrefix(technique) ?? ResourceDictionary.Get("TechniqueSelectionPage_NoHodokuPrefix", App.CurrentCulture);

	public static string GetStringResourceViaFeature(TechniqueFeature feature)
		=> feature switch
		{
			TechniqueFeature.HardToBeGenerated => ResourceDictionary.Get("TechniqueFeature_HardToBeGeneratedShort", App.CurrentCulture),
			TechniqueFeature.WillBeReplacedByOtherTechnique => ResourceDictionary.Get("TechniqueFeature_WillBeReplacedByOtherTechniqueShort", App.CurrentCulture),
			TechniqueFeature.OnlyExistInTheory => ResourceDictionary.Get("TechniqueFeature_OnlyExistInTheoryShort", App.CurrentCulture),
			TechniqueFeature.NotImplemented => ResourceDictionary.Get("TechniqueFeature_NotImplementedShort", App.CurrentCulture),
			TechniqueFeature.DirectTechniques => ResourceDictionary.Get("TechniqueFeature_DirectTechniquesShort", App.CurrentCulture),
			_ => string.Empty
		};

	public static string? GetStringTooltipViaFeature(TechniqueFeature feature)
		=> feature switch
		{
			TechniqueFeature.HardToBeGenerated => ResourceDictionary.Get("TechniqueFeature_HardToBeGenerated", App.CurrentCulture),
			TechniqueFeature.WillBeReplacedByOtherTechnique => ResourceDictionary.Get("TechniqueFeature_WillBeReplacedByOtherTechnique", App.CurrentCulture),
			TechniqueFeature.OnlyExistInTheory => ResourceDictionary.Get("TechniqueFeature_OnlyExistInTheory", App.CurrentCulture),
			TechniqueFeature.NotImplemented => ResourceDictionary.Get("TechniqueFeature_NotImplemented", App.CurrentCulture),
			TechniqueFeature.DirectTechniques => ResourceDictionary.Get("TechniqueFeature_DirectTechniques", App.CurrentCulture),
			_ => null
		};

	public static Visibility GetExtraDescriptionVisibility(TechniqueFeature feature)
		=> feature == TechniqueFeature.None ? Visibility.Collapsed : Visibility.Visible;

	public static Visibility GetFeatureVisibility(Technique technique) => GetExtraDescriptionVisibility(technique.GetFeature());

	public static Brush GetBrush(TechniqueFeature feature)
		=> new SolidColorBrush(feature == TechniqueFeature.None ? Colors.Black : Colors.LightGray);

	public static string[] GetIntroductionLinks(Technique technique) => technique.GetReferenceLinks();
}
