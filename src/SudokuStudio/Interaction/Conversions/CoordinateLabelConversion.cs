using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Sudoku.Text.Coordinate;
using SudokuStudio.Rendering;
using Windows.UI;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about coordinate labels.
/// </summary>
internal static class CoordinateLabelConversion
{
	public static double GetFontSize(double globalFontSize, decimal scale) => globalFontSize * (double)scale;

	public static string ToCoordinateLabelText(ConceptNotationBased based, int index, bool isRow)
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		switch (based)
		{
			case ConceptNotationBased.LiteralBased:
			{
				return (index + 1).ToString();
			}
			case ConceptNotationBased.RxCyBased:
			{
				var upperRxCy = uiPref.MakeLettersUpperCaseInRxCyNotation;
				var label = (isRow, upperRxCy) switch { (true, true) => 'R', (true, _) => 'r', (false, true) => 'C', _ => 'c' };
				var digit = (index + 1).ToString();
				return $"{label}{digit}";
			}
			case ConceptNotationBased.K9Based:
			{
				var upperK9 = uiPref.MakeLettersUpperCaseInK9Notation;
				var finalChar = uiPref.FinalRowLetterInK9Notation;
				var label = index == 8 ? finalChar : (char)(index + 'A');
				label = upperK9 ? char.ToUpper(label) : char.ToLower(label);
				var digit = (index + 1).ToString();
				return $"{label}{digit}";
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
