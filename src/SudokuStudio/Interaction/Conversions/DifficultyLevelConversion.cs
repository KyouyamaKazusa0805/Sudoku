namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about difficulty level title label.
/// </summary>
internal static class DifficultyLevelConversion
{
	public static string GetName(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			DifficultyLevel.Easy => GetString("_DifficultyLevel_Easy"),
			DifficultyLevel.Moderate => GetString("_DifficultyLevel_Moderate"),
			DifficultyLevel.Hard => GetString("_DifficultyLevel_Hard"),
			DifficultyLevel.Fiendish => GetString("_DifficultyLevel_Fiendish"),
			DifficultyLevel.Nightmare => GetString("_DifficultyLevel_Nightmare"),
			DifficultyLevel.LastResort => GetString("_DifficultyLevel_LastResort"),
			_ => string.Empty
		};

	public static string GetNameWithDefault(DifficultyLevel difficultyLevel, string defaultValue)
		=> difficultyLevel switch
		{
			DifficultyLevel.Easy => GetString("_DifficultyLevel_Easy"),
			DifficultyLevel.Moderate => GetString("_DifficultyLevel_Moderate"),
			DifficultyLevel.Hard => GetString("_DifficultyLevel_Hard"),
			DifficultyLevel.Fiendish => GetString("_DifficultyLevel_Fiendish"),
			DifficultyLevel.Nightmare => GetString("_DifficultyLevel_Nightmare"),
			DifficultyLevel.LastResort => GetString("_DifficultyLevel_LastResort"),
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

	public static SolidColorBrush GetForegroundColor(DifficultyLevel difficultyLevel) => new(GetForegroundRawColor(difficultyLevel));
}
