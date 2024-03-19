namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents an attribute type that will be applied to fields defined in <see cref="Technique"/>,
/// describing its detail which can be defined as fixed one, to be stored as metadata.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class TechniqueMetadataAttribute : ProgramMetadataAttribute<double, DifficultyLevel>
{
	/// <summary>
	/// Indicates whether the current technique supports for Siamese logic.
	/// </summary>
	public bool SupportsSiamese { get; init; }

	/// <summary>
	/// Indicates whether the current technique supports for Dual logic.
	/// </summary>
	public bool SupportsDual { get; init; }

	/// <summary>
	/// Indicates the rating value defined in direct mode.
	/// </summary>
	public double DirectRating { get; init; }

	/// <summary>
	/// Indicates the abbreviation of the technique.
	/// </summary>
	[DisallowNull]
	public string? Abbreviation { get; init; }

	/// <summary>
	/// Indicates the reference links.
	/// </summary>
	[StringSyntax(StringSyntax.Uri)]
	[DisallowNull]
	public string[]? Links { get; init; }

	/// <summary>
	/// Indicates the extra difficulty factors.
	/// </summary>
	[DisallowNull]
	public string[]? ExtraFactors { get; init; }

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
	/// Indicates the features of the technique.
	/// </summary>
	public TechniqueFeature Features { get; init; }

	/// <summary>
	/// Indicates the primarily-supported <see cref="Step"/> type.
	/// </summary>
	public Type? PrimarySupportedType { get; init; }

	/// <summary>
	/// Indicates the secondarily-supported <see cref="Step"/> type.
	/// </summary>
	[DisallowNull]
	public Type? SecondarySupportedType { get; init; }
}
