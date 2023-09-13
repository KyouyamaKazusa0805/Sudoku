using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Sudoku.Concepts;
using SudokuStudio.Rendering;
using Windows.UI;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about drawing.
/// </summary>
internal static class DrawingConversion
{
	public static int GetDrawingModeIndex(DrawingMode drawingMode) => (int)(drawingMode - 1);

	public static int GetLinkTypeIndex(Inference inference)
		=> inference switch
		{
			Inference.Strong => 0,
			Inference.Weak => 1,
			Inference.Default => 2
		};

	public static Visibility GetColorPaletteSelectorVisibility(DrawingMode drawingMode)
		=> drawingMode switch
		{
			DrawingMode.Cell or DrawingMode.Candidate or DrawingMode.Chute or DrawingMode.House or DrawingMode.BabaGrouping => Visibility.Visible,
			_ => Visibility.Collapsed
		};

	public static Visibility GetLinkKindSelectorVisibility(DrawingMode drawingMode)
		=> drawingMode == DrawingMode.Link ? Visibility.Visible : Visibility.Collapsed;

	public static Visibility GetBabaGroupVisibility(DrawingMode drawingMode)
		=> drawingMode == DrawingMode.BabaGrouping ? Visibility.Visible : Visibility.Collapsed;

	public static Brush GetBrush(Color color) => new SolidColorBrush(color);

	public static Brush GetSelectedBrush(int currentColorIndex)
		=> ((App)Application.Current).Preference.UIPreferences.UserDefinedColorPalette switch
		{
			{ Count: var countOfColors } palette when currentColorIndex >= 0 && currentColorIndex < countOfColors
				=> new SolidColorBrush(palette[currentColorIndex]),
			_ => new SolidColorBrush()
		};
}
