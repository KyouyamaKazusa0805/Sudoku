namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents an attribute type that will be applied to fields defined in <see cref="Technique"/>,
/// describing its detail which can be defined as fixed one, to be stored as metadata.
/// </summary>
/// <param name="rating">Indicates the rating value.</param>
/// <param name="difficultyLevel">Indicates the difficulty level for the current technique.</param>
/// <param name="containingGroup">Indicates the group that the current technique belongs to.</param>
/// <param name="primarySupportedType">Indicates the primarily-supported <see cref="Step"/> type.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class TechniqueMetadataAttribute(
	[PrimaryConstructorParameter(Accessibility = "public override")] double rating = double.MinValue,
	[PrimaryConstructorParameter(Accessibility = "public override")] DifficultyLevel difficultyLevel = DifficultyLevel.Unknown,
	[PrimaryConstructorParameter] TechniqueGroup containingGroup = TechniqueGroup.None,
	[PrimaryConstructorParameter] Type? primarySupportedType = null
) : ProgramMetadataAttribute<double, DifficultyLevel>
{
	/// <summary>
	/// Indicates whether the current technique supports for Siamese logic.
	/// </summary>
	public bool SupportsSiamese { get; set; }

	/// <summary>
	/// Indicates whether the current technique supports for Dual logic.
	/// </summary>
	public bool SupportsDual { get; set; }

	/// <summary>
	/// Indicates the rating value defined in direct mode.
	/// </summary>
	public double DirectRating { get; set; }

	/// <summary>
	/// Indicates the abbreviation of the technique.
	/// </summary>
	[DisallowNull]
	public string? Abbreviation { get; set; }

	/// <summary>
	/// Indicates the reference links.
	/// </summary>
	[StringSyntax(StringSyntax.Uri)]
	[DisallowNull]
	public string[]? Links { get; set; }

	/// <summary>
	/// Indicates the extra difficulty factors.
	/// </summary>
	[DisallowNull]
	public string[]? ExtraFactors { get; set; }

	/// <summary>
	/// Indicates the mode that the current technique can be used by solving a puzzle.
	/// By default the value is both <see cref="PencilmarkVisibility.Direct"/> and <see cref="PencilmarkVisibility.Indirect"/>.
	/// </summary>
	/// <seealso cref="PencilmarkVisibility.Direct"/>
	/// <seealso cref="PencilmarkVisibility.Indirect"/>
	public PencilmarkVisibility PencilmarkVisibility { get; set; } = PencilmarkVisibility.Direct | PencilmarkVisibility.Indirect;

	/// <summary>
	/// Indicates the features of the technique.
	/// </summary>
	public TechniqueFeature Features { get; set; }

	/// <summary>
	/// Indicates the secondarily-supported <see cref="Step"/> type.
	/// </summary>
	[DisallowNull]
	public Type? SecondarySupportedType { get; set; }
}
