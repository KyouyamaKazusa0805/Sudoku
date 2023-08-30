namespace SudokuStudio.Interaction.Conversions;

internal static class GeneratingStrategyConversion
{
	public static string GetTitle(string titleKey) => GetString(titleKey);

	public static double GetEditButtonOpacity(bool isEditing, bool isHovered) => isHovered && !isEditing ? 1 : 0;

	public static double GetSubmitButtonOpacity(bool isEditing, bool isHovered) => isHovered && isEditing ? 1 : 0;
}
