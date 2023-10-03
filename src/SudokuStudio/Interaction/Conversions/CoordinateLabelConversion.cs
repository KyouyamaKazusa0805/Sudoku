using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Sudoku.Concepts;
using SudokuStudio.Rendering;
using Windows.UI;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about coordinate labels.
/// </summary>
internal static class CoordinateLabelConversion
{
	public static double GetFontSize(double globalFontSize, decimal scale) => globalFontSize * (double)scale;

	public static string ToCoordinateLabelText(CoordinateType based, int index, bool isRow)
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		switch (based)
		{
			case CoordinateType.Literal:
			{
				return (index + 1).ToString();
			}
			case CoordinateType.RxCy:
			{
				var upperRxCy = uiPref.MakeLettersUpperCaseInRxCyNotation;
				var label = (isRow, upperRxCy) switch { (true, true) => 'C', (true, _) => 'c', (false, true) => 'R', _ => 'r' };
				var digit = (index + 1).ToString();
				return $"{label}{digit}";
			}
			case CoordinateType.K9:
			{
				var label = (index == 8 ? uiPref.FinalRowLetterInK9Notation : (char)(index + 'A')) is var l
					&& uiPref.MakeLettersUpperCaseInK9Notation ? char.ToUpper(l) : char.ToLower(l);
				return isRow ? (index + 1).ToString() : label.ToString();
			}
			case CoordinateType.Excel:
			{
				var label = (char)(index + (uiPref.MakeLettersUpperCaseInExcelNotation ? 'A' : 'a'));
				return isRow ? label.ToString() : (index + 1).ToString();
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(based));
			}
		}
	}

	public static Visibility ToCoordinateLabelVisibility(CoordinateLabelDisplayMode mode)
		=> mode == CoordinateLabelDisplayMode.None ? Visibility.Collapsed : Visibility.Visible;

	public static Visibility ToCoordinateLabelVisibilityLower(CoordinateLabelDisplayMode mode)
		=> mode == CoordinateLabelDisplayMode.FourDirection ? Visibility.Visible : Visibility.Collapsed;

	public static Brush GetBrush(Color color) => new SolidColorBrush(color);
}
