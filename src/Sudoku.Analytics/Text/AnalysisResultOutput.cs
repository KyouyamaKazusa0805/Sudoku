namespace Sudoku.Text;

/// <summary>
/// Provides an easy way to output <see cref="AnalysisResult"/> with console colorization.
/// </summary>
public static class AnalysisResultOutput
{
	/// <summary>
	/// Gets the output text.
	/// </summary>
	/// <param name="instance">The instance.</param>
	/// <param name="options">The options.</param>
	/// <param name="formatProvider">The format provider.</param>
	/// <returns>The string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetColorizedText(
		AnalysisResult instance,
		AnalysisResult.FormattingOptions options,
		IFormatProvider? formatProvider
	) => instance.ToString(
		options,
		formatProvider,
		static (str, step) =>
		{
			var @default = (-1, -1, -1);
			var c = step.DifficultyLevel switch
			{
				DifficultyLevel.Moderate => (0, 255, 0),
				DifficultyLevel.Hard => (255, 255, 0),
				DifficultyLevel.Fiendish => (255, 150, 80),
				DifficultyLevel.Nightmare => (255, 100, 100),
				_ => @default
			};
			return c == @default ? str : $"\e[38;2;{c.Item1};{c.Item2};{c.Item3}m{str}\e[0m";
		}
	);
}
