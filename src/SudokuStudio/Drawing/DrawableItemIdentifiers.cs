namespace SudokuStudio.Drawing;

/// <summary>
/// Represents a list of identifiers used by renderable items that will be consumed by <see cref="DrawableFactory"/>.
/// </summary>
/// <seealso cref="DrawableFactory"/>
internal static class DrawableItemIdentifiers
{
	/// <summary>
	/// Indicates the separator for the suffix of conclusions in tag text.
	/// </summary>
	public const string ConclusionSuffixSeparator = "|c";

	/// <summary>
	/// Indicates the separator for the suffix of inferences in tag text.
	/// </summary>
	public const string InferenceSuffixSeparator = "|i";

	/// <summary>
	/// Indicates the separator for the suffix of colorized in tag text.
	/// </summary>
	public const string ColorizedSuffixSeparator = "|@";

	/// <summary>
	/// Indicates the separator for the suffix of colorized color identifier in tag text.
	/// </summary>
	public const string ColorColorIdentifierSeparator = "|*";

	/// <summary>
	/// Indicates the separator for the suffix of colorized named identifier in tag text.
	/// </summary>
	public const string IdColorIdentifierSeparator = "|#";

	/// <summary>
	/// The conclusion suffixes.
	/// </summary>
	public const string
		CannibalismConclusionSuffix = "|cc|",
		EliminationConclusionSuffix = "|ce|",
		OverlappedAssignmentConclusionSuffix = "|co|",
		AssignmentConclusionSuffix = "|ca|";

	/// <summary>
	/// The link suffixes.
	/// </summary>
	public const string
		StrongInferenceSuffix = "|is|",
		StrongGeneralizedInferenceSuffix = "|isg|",
		WeakInferenceSuffix = "|iw|",
		WeakGeneralizedInferenceSuffix = "|iwg|",
		ConjugateInferenceSuffix = "|ic|",
		DefaultInferenceSuffix = "|id|";

	/// <summary>
	/// The colorized suffixes.
	/// </summary>
	public const string
		NormalColorizedSuffix = $"|@{nameof(ColorIdentifierKind.Normal)}|",
		AssignmentColorizedSuffix = $"|@{nameof(ColorIdentifierKind.Assignment)}|",
		OverlappedAssignmentColorizedSuffix = $"|@{nameof(ColorIdentifierKind.OverlappedAssignment)}|",
		EliminationColorizedSuffix = $"|@{nameof(ColorIdentifierKind.Elimination)}|",
		CannibalismColorizedSuffix = $"|@{nameof(ColorIdentifierKind.Cannibalism)}|",
		LinkColorizedSuffix = $"|@{nameof(ColorIdentifierKind.Link)}|",
		ExofinColorizedSuffix = $"|@{nameof(ColorIdentifierKind.Exofin)}|",
		EndofinColorizedSuffix = $"|@{nameof(ColorIdentifierKind.Endofin)}|",
		AuxiliaryColorizedSuffix = "|@Auxiliary|",
		AlmostLockedSetColorizedSuffix = "|@AlmostLockedSet|",
		ColorPaletteColorizedSuffix = "|@ColorPalette|";
}
