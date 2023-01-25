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

	public static SolidColorBrush GetBackgroundColor(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			0 or > DifficultyLevel.LastResort => Constants.Backgrounds[^1],
			_ => Constants.Backgrounds[Log2((byte)difficultyLevel)]
		};

	public static SolidColorBrush GetForegroundColor(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			0 or > DifficultyLevel.LastResort => new(Colors.Transparent),
			_ => Constants.Foregrounds[Log2((byte)difficultyLevel)]
		};
}

/// <include file='../../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="constant"]'/>
file static class Constants
{
	public static readonly SolidColorBrush[] Foregrounds =
	{
		new(Color.FromArgb(255,   0,  51, 204)),
		new(Color.FromArgb(255,   0, 102,   0)),
		new(Color.FromArgb(255, 102,  51,   0)),
		new(Color.FromArgb(255, 102,  51,   0)),
		new(Color.FromArgb(255, 102,   0,   0)),
		new(Colors.Black)
	};

	public static readonly SolidColorBrush[] Backgrounds =
	{
		new(Color.FromArgb(255, 204, 204, 255)),
		new(Color.FromArgb(255, 100, 255, 100)),
		new(Color.FromArgb(255, 255, 255, 100)),
		new(Color.FromArgb(255, 255, 150,  80)),
		new(Color.FromArgb(255, 255, 100, 100)),
		new(Color.FromArgb(255, 220, 220, 220))
	};
}
