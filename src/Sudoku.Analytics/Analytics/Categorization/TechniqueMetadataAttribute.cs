namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents an attribute type that will be applied to fields defined in <see cref="Technique"/>,
/// describing its detail which can be defined as fixed one, to be stored as metadata.
/// </summary>
/// <shared-comments>
/// <para>
/// <b>This property can only be applied to a <see cref="TechniqueGroup"/> field.</b>
/// </para>
/// </shared-comments>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class TechniqueMetadataAttribute : ProgramMetadataAttribute<int, DifficultyLevel>
{
	/// <summary>
	/// Indicates whether the current technique supports for Siamese logic.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="TechniqueMetadataAttribute" path="//shared-comments/para[1]"/>
	/// </remarks>
	public bool SupportsSiamese { get; init; }

	/// <summary>
	/// Indicates whether the current technique supports for Dual logic.
	/// </summary>
	/// <remarks><inheritdoc cref="TechniqueMetadataAttribute" path="//shared-comments/para[1]"/></remarks>
	public bool SupportsDual { get; init; }

	/// <summary>
	/// Indicates the rating value defined in direct mode.
	/// </summary>
	public int DirectRating { get; init; }

	/// <summary>
	/// Indicates the resource key that can fetch the corresponding resource string.
	/// </summary>
	[DisallowNull]
	public string? ResourceKey { get; init; }

	/// <summary>
	/// Indicates the abbreviation of the technique.
	/// </summary>
	[DisallowNull]
	public string? Abbreviation { get; init; }

	/// <summary>
	/// Indicates the reference links.
	/// </summary>
	[StringSyntax(StringSyntaxAttribute.Uri)]
	[DisallowNull]
	public string[]? Links { get; init; }

	/// <summary>
	/// Indicates the containing techniuqe group that the current technique belongs to.
	/// </summary>
	public TechniqueGroup ContainingGroup { get; init; }

	/// <summary>
	/// Indicates the mode that the current technique can be used by solving a puzzle.
	/// By default the value is both <see cref="PencilmarkVisibility.Direct"/> and <see cref="PencilmarkVisibility.Indirect"/>.
	/// </summary>
	/// <seealso cref="PencilmarkVisibility.Direct"/>
	/// <seealso cref="PencilmarkVisibility.Indirect"/>
	public PencilmarkVisibility PencilmarkVisibility { get; init; } = PencilmarkVisibility.Direct | PencilmarkVisibility.Indirect;

	/// <summary>
	/// Indicates the customized related technique that the current technique is applied.
	/// </summary>
	public Technique RelatedTechnique { get; init; }

	/// <summary>
	/// Indicates the features of the technique.
	/// </summary>
	public TechniqueFeatures Features { get; init; }

	/// <summary>
	/// Indicates the special flags that the current technique will be applied in metadata.
	/// </summary>
	public TechniqueMetadataSpecialFlags SpecialFlags { get; init; }

	/// <summary>
	/// Indicates a that that can create a <see cref="Step"/> instance that includes the current technique usage.
	/// </summary>
	[DisallowNull]
	public Type? StepType { get; init; }

	/// <summary>
	/// Indicates a step searcher type that can produce steps that describes the current technique.
	/// </summary>
	[DisallowNull]
	public Type? StepSearcherType { get; init; }
}
