namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about difficulty level title label.
/// </summary>
internal static class DifficultyLevelConversion
{
	public static string GetName(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			DifficultyLevel.Easy => ResourceDictionary.Get("_DifficultyLevel_Easy", App.CurrentCulture),
			DifficultyLevel.Moderate => ResourceDictionary.Get("_DifficultyLevel_Moderate", App.CurrentCulture),
			DifficultyLevel.Hard => ResourceDictionary.Get("_DifficultyLevel_Hard", App.CurrentCulture),
			DifficultyLevel.Fiendish => ResourceDictionary.Get("_DifficultyLevel_Fiendish", App.CurrentCulture),
			DifficultyLevel.Nightmare => ResourceDictionary.Get("_DifficultyLevel_Nightmare", App.CurrentCulture),
			DifficultyLevel.LastResort => ResourceDictionary.Get("_DifficultyLevel_LastResort", App.CurrentCulture),
			_ => string.Empty
		};

	public static string GetNameWithDefault(DifficultyLevel difficultyLevel, string defaultValue)
		=> difficultyLevel switch
		{
			DifficultyLevel.Easy => ResourceDictionary.Get("_DifficultyLevel_Easy", App.CurrentCulture),
			DifficultyLevel.Moderate => ResourceDictionary.Get("_DifficultyLevel_Moderate", App.CurrentCulture),
			DifficultyLevel.Hard => ResourceDictionary.Get("_DifficultyLevel_Hard", App.CurrentCulture),
			DifficultyLevel.Fiendish => ResourceDictionary.Get("_DifficultyLevel_Fiendish", App.CurrentCulture),
			DifficultyLevel.Nightmare => ResourceDictionary.Get("_DifficultyLevel_Nightmare", App.CurrentCulture),
			DifficultyLevel.LastResort => ResourceDictionary.Get("_DifficultyLevel_LastResort", App.CurrentCulture),
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
