namespace Sudoku.UI.Models;

/// <summary>
/// Defines a logical step.
/// </summary>
public sealed partial class LogicalStep
{
	/// <summary>
	/// Indicates a <see cref="BindingFlags"/> instance that binds with all possible members stored in a type.
	/// This constant is used for reflection.
	/// </summary>
	private const BindingFlags AllMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;


	/// <summary>
	/// Indicates the visibility of extra feature badge.
	/// </summary>
	public Visibility ExtraFeatureBadgeVisibility
		=> ExtraFeatures != StepDisplayingFeature.None ? Visibility.Visible : Visibility.Collapsed;

	/// <summary>
	/// Indicates the extra displaying features.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when:
	/// <list type="bullet">
	/// <item>
	/// The specified member given in member <see cref="StepDisplayingFeatureAttribute.VerifyMemberName"/>
	/// cannot be found.
	/// </item>
	/// <item>Multiple members can be matched.</item>
	/// <item>
	/// The value given in member <see cref="StepDisplayingFeatureAttribute.VerifyMemberName"/> matches a method,
	/// but the method returns <see langword="void"/>.
	/// </item>
	/// <item>
	/// Values <see cref="StepDisplayingFeatureAttribute.VerifyMemberName"/> and
	/// <see cref="StepDisplayingFeatureAttribute.VerifyMemberValue"/> hold invalid values
	/// (e.g. one is <see langword="null"/> but the other is not <see langword="null"/>).
	/// </item>
	/// </list>
	/// </exception>
	/// <exception cref="NotSupportedException">
	/// Throws when:
	/// <list type="bullet">
	/// <item>
	/// The value given in member <see cref="StepDisplayingFeatureAttribute.VerifyMemberName"/> matches a method,
	/// but the method requires at least one parameter.
	/// </item>
	/// <item>
	/// The value given in member <see cref="StepDisplayingFeatureAttribute.VerifyMemberName"/> matches a 
	/// </item>
	/// </list>
	/// </exception>
	/// <seealso cref="StepDisplayingFeatureAttribute"/>
	public StepDisplayingFeature ExtraFeatures
		=> Step.GetType() switch
		{
			var type => type.GetCustomAttribute<StepDisplayingFeatureAttribute>() switch
			{
				null => StepDisplayingFeature.None,
				{ Features: var features, VerifyMemberName: null, VerifyMemberValue: null } => features,
				{ Features: var features, VerifyMemberName: { } memberName, VerifyMemberValue: { } value }
					=> type.GetMember(memberName, AllMembers) switch
					{
						[] => throw new InvalidOperationException($"The value is invalid: The specified member '{memberName}' cannot be found."),
						{ Length: >= 2 } => throw new InvalidOperationException($"The value is invalid: Multiple members matched: '{memberName}', cannot remove ambiguity."),
						[var memberInfo] => memberInfo switch
						{
							FieldInfo fi => value.Equals(fi.GetValue(Step)) ? features : StepDisplayingFeature.None,
							PropertyInfo pi => value.Equals(pi.GetValue(Step)) ? features : StepDisplayingFeature.None,
							MethodInfo mi => mi switch
							{
								{ ReturnType: var returnType } when returnType == typeof(void) => throw new InvalidOperationException($"The value is invalid: The specified matched method '{memberName}' returns void."),
								_ when mi.GetParameters() is not [] => throw new NotSupportedException($"The specified method is not supported because it requires at least one parameter."),
								_ => value.Equals(mi.Invoke(Step, null)) ? features : StepDisplayingFeature.None,
							},
							_ => throw new NotSupportedException("The matched member can only be supported fields, properties and parameterless methods returning non-void types.")
						}
					},
				_ => throw new InvalidOperationException("The value is invalid in the attribute.")
			}
		};

	/// <summary>
	/// Indicates the current grid used.
	/// </summary>
	public required Grid Grid { get; set; }

	/// <summary>
	/// Indicates the step.
	/// </summary>
	public required IStep Step { get; set; }

	[GeneratedDeconstruction]
	public partial void Deconstruct(out Grid grid, out IStep step);

	/// <summary>
	/// Gets the display string value that can describe the main information of the current step.
	/// </summary>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToDisplayString()
	{
		var openBrace = R["Token_OpenBrace"]!;
		var closedBrace = R["Token_ClosedBrace"]!;
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
			StepDisplayingFeature.None
				=> string.Empty,
			StepDisplayingFeature.HideDifficultyRating
				=> R["SudokuPage_InfoBadge_ThisTechniqueDoesNotShowDifficulty"]!,
			StepDisplayingFeature.VeryRare
				=> R["SudokuPage_InfoBadge_ThisTechniqueIsVeryRare"]!,
			StepDisplayingFeature.DifficultyRatingNotStable
				=> R["SudokuPage_InfoBadge_ThisTechniqueDiffcultyRatingIsNotStable"]!,
			StepDisplayingFeature.VeryRare | StepDisplayingFeature.DifficultyRatingNotStable
				=> R["SudokuPage_InfoBadge_ThisTechniqueIsVeryRare"]!,
			StepDisplayingFeature.ConstructedTechnique
				=> R["SudokuPage_InfoBadge_ConstructedTechnique"]!,
			StepDisplayingFeature.DifficultyRatingNotStable | StepDisplayingFeature.ConstructedTechnique
				=> R["SudokuPage_InfoBadge_ConstructedTechnique"]!,
			_
				=> string.Empty
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
			StepDisplayingFeature.DifficultyRatingNotStable => new SolidColorBrush(Colors.Purple),
			StepDisplayingFeature.VeryRare | StepDisplayingFeature.DifficultyRatingNotStable
				=> new LinearGradientBrush()
					.WithStartPoint(0, .5)
					.WithEndPoint(1, .5)
					.WithGradientStops(
						new GradientStop()
							.WithColor(Colors.SkyBlue),
						new GradientStop()
							.WithColor(Colors.Purple)
					),
			StepDisplayingFeature.ConstructedTechnique => new SolidColorBrush(Colors.Green),
			StepDisplayingFeature.DifficultyRatingNotStable | StepDisplayingFeature.ConstructedTechnique
				=> new LinearGradientBrush()
					.WithStartPoint(0, .5)
					.WithEndPoint(1, .5)
					.WithGradientStops(
						new GradientStop()
							.WithColor(Colors.Green),
						new GradientStop()
							.WithColor(Colors.Purple)
					),
			_ => null
		};
}
