using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SudokuStudio.Collection;
using SudokuStudio.Rendering;
using Windows.Foundation;
using Windows.UI;

namespace SudokuStudio.Interaction.Conversions;

internal static class SettingsPageConversion
{
	public static Offset GetSelectedIndexForEmptyCellCharacter(ComboBox comboBox)
	{
		var character = ((App)Application.Current).Preference.UIPreferences.EmptyCellCharacter;
		var i = 0;
		foreach (var element in comboBox.Items.Cast<ComboBoxItem>())
		{
			if (element.Tag is string and [var ch] && ch == character)
			{
				return i;
			}

			i++;
		}

		return -1;
	}

	public static string GetDashArrayString(DashArray value) => value.ToString()[1..^1];

	public static string GetSliderString(double value, string format) => value.ToString(format);

	public static string ToRgbString(Color color)
	{
		var (r, g, b) = color;
		return $"{r}, {g}, {b}";
	}

	public static string ToArgbString(Color color)
	{
		var (a, r, g, b) = color;
		return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
	}

	public static string ToArgbRecordLikeString(Color color)
	{
		var (a, r, g, b) = color;
		return $"{nameof(Color.A)} = {a} {nameof(Color.R)} = {r} {nameof(Color.G)} = {g} {nameof(Color.B)} = {b}";
	}

	public static CoordinateLabelDisplayMode GetCoordinateLabelDisplayMode(Offset index) => (CoordinateLabelDisplayMode)index;

	public static Visibility GetVisibility(string? text) => string.IsNullOrWhiteSpace(text) ? Visibility.Collapsed : Visibility.Visible;

	public static Brush GetBrush(Color color) => new SolidColorBrush(color);

	public static FontFamily GetFont(string fontName) => new(fontName);
}
