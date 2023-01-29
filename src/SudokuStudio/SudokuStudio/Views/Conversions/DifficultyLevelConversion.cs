namespace SudokuStudio.Views.Conversions;

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

	public static Color GetBackgroundRawColor(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			0 or > DifficultyLevel.LastResort => Constants.Backgrounds[^1],
			_ => Constants.Backgrounds[Log2((byte)difficultyLevel)]
		};

	public static Color GetForegroundRawColor(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			0 or > DifficultyLevel.LastResort => Constants.Foregrounds[^1],
			_ => Constants.Foregrounds[Log2((byte)difficultyLevel)]
		};

	public static SolidColorBrush GetBackgroundColor(DifficultyLevel difficultyLevel) => new(GetBackgroundRawColor(difficultyLevel));

	public static SolidColorBrush GetForegroundColor(DifficultyLevel difficultyLevel) => new(GetForegroundRawColor(difficultyLevel));
}

/// <include file='../../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="constant"]'/>
file static class Constants
{
	public static readonly Color[] Foregrounds =
	{
		Color.FromArgb(255,   0,  51, 204),
		Color.FromArgb(255,   0, 102,   0),
		Color.FromArgb(255, 102,  51,   0),
		Color.FromArgb(255, 102,  51,   0),
		Color.FromArgb(255, 102,   0,   0),
		Colors.Black
	};

	public static readonly Color[] Backgrounds =
	{
		Color.FromArgb(255, 204, 204, 255),
		Color.FromArgb(255, 100, 255, 100),
		Color.FromArgb(255, 255, 255, 100),
		Color.FromArgb(255, 255, 150,  80),
		Color.FromArgb(255, 255, 100, 100),
		Color.FromArgb(255, 220, 220, 220)
	};
}
