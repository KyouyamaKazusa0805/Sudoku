using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Sudoku.Analytics.Categorization;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about technique.
/// </summary>
internal static class TechniqueConversion
{
	public static int GetDisplayNameColumnSpan(TechniqueFeature feature) => feature == TechniqueFeature.None ? 2 : 1;

	public static string GetStringResourceViaFeature(TechniqueFeature feature)
		=> feature switch
		{
			TechniqueFeature.HardToBeGenerated => GetString("TechniqueFeature_HardToBeGeneratedShort"),
			TechniqueFeature.WillBeReplacedByOtherTechnique => GetString("TechniqueFeature_WillBeReplacedByOtherTechniqueShort"),
			TechniqueFeature.OnlyExistInTheory => GetString("TechniqueFeature_OnlyExistInTheoryShort"),
			TechniqueFeature.NotImplemented => GetString("TechniqueFeature_NotImplementedShort"),
			TechniqueFeature.DirectTechniques => GetString("TechniqueFeature_DirectTechniquesShort"),
			_ => string.Empty
		};

	public static string? GetStringTooltipViaFeature(TechniqueFeature feature)
		=> feature switch
		{
			TechniqueFeature.HardToBeGenerated => GetString("TechniqueFeature_HardToBeGenerated"),
			TechniqueFeature.WillBeReplacedByOtherTechnique => GetString("TechniqueFeature_WillBeReplacedByOtherTechnique"),
			TechniqueFeature.OnlyExistInTheory => GetString("TechniqueFeature_OnlyExistInTheory"),
			TechniqueFeature.NotImplemented => GetString("TechniqueFeature_NotImplemented"),
			TechniqueFeature.DirectTechniques => GetString("TechniqueFeature_DirectTechniques"),
			_ => null
		};

	public static Visibility GetExtraDescriptionVisibility(TechniqueFeature feature)
		=> feature == TechniqueFeature.None ? Visibility.Collapsed : Visibility.Visible;

	public static Brush GetBrush(TechniqueFeature feature)
		=> new SolidColorBrush(feature == TechniqueFeature.None ? Colors.Black : Colors.LightGray);
}
