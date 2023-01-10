namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about coordinate labels.
/// </summary>
internal static class CoordinateLabelConversion
{
	public static string ToCoordinateLabelText(CoordinateLabelDisplayKind coordinateKind, int index, bool isRow)
		=> coordinateKind switch
		{
			CoordinateLabelDisplayKind.None => "/",
			CoordinateLabelDisplayKind.RxCy => $"{(isRow ? 'c' : 'r')}{index + 1}",
			CoordinateLabelDisplayKind.K9 => $"{(isRow ? (char)(index + '1') : (char)(index + 'A'))}"
		};

	public static Visibility ToCoordinateLabelVisibility(CoordinateLabelDisplayKind coordinateKind)
		=> coordinateKind == CoordinateLabelDisplayKind.None ? Visibility.Collapsed : Visibility.Visible;

	public static Brush GetBrush(Color color) => new SolidColorBrush(color);
}
