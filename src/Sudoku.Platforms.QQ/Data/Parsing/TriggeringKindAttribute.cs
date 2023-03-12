namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Defines a triggering kind attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TriggeringKindAttribute : CommandLineParsingItemAttribute
{
	/// <summary>
	/// Initializes a <see cref="TriggeringKindAttribute"/> instance via the specified kind.
	/// </summary>
	/// <param name="kind">The kind.</param>
	public TriggeringKindAttribute(ModuleTriggeringKind kind) => Kind = kind;


	/// <summary>
	/// Indicates the triggering kind.
	/// </summary>
	public ModuleTriggeringKind Kind { get; }
}
