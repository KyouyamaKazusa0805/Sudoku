namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about analyze tab pages.
/// </summary>
internal static class AnalyzeConversion
{
	public static bool GetIsEnabled(Grid grid) => !grid.GetSolution().IsUndefined;

	public static string GetEliminationString(IStep step) => ConclusionFormatter.Format(step.Conclusions.ToArray(), FormattingMode.Normal);

	public static string GetDifficultyRatingText(IStep step) => step.Difficulty.ToString("0.0");

	public static Visibility GetDifficultyRatingVisibility(bool showDifficultyRating)
		=> showDifficultyRating ? Visibility.Visible : Visibility.Collapsed;

	public static Visibility GetSummaryTableVisibility(IEnumerable itemsSource)
		=> itemsSource is null || itemsSource.None() ? Visibility.Collapsed : Visibility.Visible;

	public static Visibility GetSolvingPathListVisibility(object itemsSource)
		=> itemsSource switch { List<IStep> and not [] => Visibility.Visible, _ => Visibility.Collapsed };
}

/// <include file='../../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Determines whether an <see cref="IEnumerable"/> collection has no elements in it.
	/// </summary>
	/// <param name="this">The element.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool None(this IEnumerable @this) => !@this.GetEnumerator().MoveNext();
}
