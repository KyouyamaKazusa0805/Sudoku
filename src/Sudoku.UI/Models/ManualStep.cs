namespace Sudoku.UI.Models;

/// <summary>
/// Defines a manual step.
/// </summary>
[AutoDeconstruction(nameof(Grid), nameof(Step))]
public sealed partial class ManualStep
{
	/// <summary>
	/// Indicates the visibility of extra feature badge.
	/// </summary>
	public Visibility ExtraFeatureBadgeVisibility
		=> ExtraFeatures != StepDisplayingFeature.None ? Visibility.Visible : Visibility.Collapsed;

	/// <summary>
	/// Indicates the extra displaying features.
	/// </summary>
	public StepDisplayingFeature ExtraFeatures
		=> DefaultExtraFeatures
		| (Step.Rarity == Rarity.OnlyForSpecialPuzzles ? StepDisplayingFeature.VeryRare : StepDisplayingFeature.None);

	/// <summary>
	/// Indicates the current grid used.
	/// </summary>
	public required Grid Grid { get; set; }

	/// <summary>
	/// Indicates the step.
	/// </summary>
	public required IStep Step { get; set; }

	/// <summary>
	/// Indicates the default extra features.
	/// </summary>
	private StepDisplayingFeature DefaultExtraFeatures
		=> Step.GetType().GetCustomAttribute<StepDisplayingFeatureAttribute>()?.Features ?? StepDisplayingFeature.None;


	/// <summary>
	/// Gets the display string value that can describe the main information of the current step.
	/// </summary>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToDisplayString()
	{
		string openBrace = R["Token_OpenBrace"]!;
		string closedBrace = R["Token_ClosedBrace"]!;
		return ExtraFeatures.Flags(StepDisplayingFeature.HideDifficultyRating)
			? Step.ToSimpleString()
			: $"{openBrace}{R["DifficultyRating"]!} {Step.Difficulty:0.0}{closedBrace} {Step.ToSimpleString()}";
	}

	/// <summary>
	/// Fetches the tool tip text.
	/// </summary>
	/// <returns>The tool tip text to be displayed.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetToolTipText()
		=> ExtraFeatures switch
		{
			StepDisplayingFeature.None => string.Empty,
			StepDisplayingFeature.HideDifficultyRating => R["SudokuPage_InfoBadge_ThisTechniqueDoesNotShowDifficulty"]!,
			StepDisplayingFeature.VeryRare => R["SudokuPage_InfoBadge_ThisTechniqueIsVeryRare"]!,
			_ => string.Empty
		};

	/// <summary>
	/// Fetches the background brush of the badge control.
	/// </summary>
	/// <returns>The background brush of the badge control.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Brush? GetBadgeBackgroundBrush()
		=> ExtraFeatures switch
		{
			StepDisplayingFeature.None => null,
			StepDisplayingFeature.HideDifficultyRating => new SolidColorBrush(Colors.Yellow),
			StepDisplayingFeature.VeryRare => new SolidColorBrush(Colors.SkyBlue),
			_ => null
		};
}
