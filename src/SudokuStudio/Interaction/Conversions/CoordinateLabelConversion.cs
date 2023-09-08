using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SudokuStudio.Rendering;
using Windows.UI;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about coordinate labels.
/// </summary>
internal static class CoordinateLabelConversion
{
	public static double GetFontSize(double globalFontSize, decimal scale) => globalFontSize * (double)scale;

	public static string ToCoordinateLabelText(CoordinateLabelDisplayKind coordinateKind, int index, bool isRow)
		=> coordinateKind switch
		{
			CoordinateLabelDisplayKind.None => "/",
			CoordinateLabelDisplayKind.RxCy => $"{(isRow ? 'c' : 'r')}{index + 1}",
			CoordinateLabelDisplayKind.K9 => $"{(isRow ? (char)(index + '1') : (char)(index + 'A'))}"
		};

	public static Visibility ToCoordinateLabelVisibility(CoordinateLabelDisplayMode mode)
		=> mode == CoordinateLabelDisplayMode.None ? Visibility.Collapsed : Visibility.Visible;

	public static Visibility ToCoordinateLabelVisibilityLower(CoordinateLabelDisplayMode mode)
		=> mode == CoordinateLabelDisplayMode.FourDirection ? Visibility.Visible : Visibility.Collapsed;

	public static Brush GetBrush(Color color) => new SolidColorBrush(color);
}
