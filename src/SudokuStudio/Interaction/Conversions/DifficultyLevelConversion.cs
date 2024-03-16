namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about difficulty level title label.
/// </summary>
internal static class DifficultyLevelConversion
{
	public static string GetName(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			not DifficultyLevel.Unknown when Enum.IsDefined(difficultyLevel) => difficultyLevel.GetName(App.CurrentCulture),
			_ => string.Empty
		};

	public static string GetNameWithDefault(DifficultyLevel difficultyLevel, string defaultValue)
		=> difficultyLevel switch
		{
			not DifficultyLevel.Unknown when Enum.IsDefined(difficultyLevel) => difficultyLevel.GetName(App.CurrentCulture),
			_ => defaultValue
		};

	public static Color GetBackgroundRawColor(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			0 or > DifficultyLevel.LastResort => ((App)Application.Current).Preference.UIPreferences.DifficultyLevelBackgrounds[^1],
			_ => ((App)Application.Current).Preference.UIPreferences.DifficultyLevelBackgrounds[Log2((byte)difficultyLevel)]
		};

	public static Color GetForegroundRawColor(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			0 or > DifficultyLevel.LastResort => ((App)Application.Current).Preference.UIPreferences.DifficultyLevelForegrounds[^1],
			_ => ((App)Application.Current).Preference.UIPreferences.DifficultyLevelForegrounds[Log2((byte)difficultyLevel)]
		};

	public static SolidColorBrush GetBackgroundColor(DifficultyLevel difficultyLevel) => new(GetBackgroundRawColor(difficultyLevel));

	public static SolidColorBrush GetBackgroundColorFromCode(Technique technique)
	{
		var pref = ((App)Application.Current).Preference.TechniqueInfoPreferences;
		return GetBackgroundColor(pref.GetDifficultyLevelOrDefault(technique));
	}

	public static SolidColorBrush GetForegroundColor(DifficultyLevel difficultyLevel) => new(GetForegroundRawColor(difficultyLevel));

	public static SolidColorBrush GetForegroundColorFromCode(Technique technique)
	{
		var pref = ((App)Application.Current).Preference.TechniqueInfoPreferences;
		return GetForegroundColor(pref.GetDifficultyLevelOrDefault(technique));
	}
}
