namespace Sudoku.Diagnostics.CodeGen;

internal static class Constants
{
	/// <summary>
	/// Indicates the version of this project.
	/// </summary>
	public const string VersionValue = "2.3";

	/// <summary>
	/// Indicates the namespace of the generator attributes.
	/// </summary>
	public const string GeneratorAttributesNamespace = "System.Diagnostics.CodeGen";

	/// <summary>
	/// Indicates the attribute name of the auto extension deconstruction.
	/// </summary>
	public const string AutoExtensionDeconstructionAttribute = nameof(AutoExtensionDeconstructionAttribute);

	/// <summary>
	/// Indicates the attribute name of the enum switch expression arm.
	/// </summary>
	public const string EnumSwitchExpressionArmAttribute = nameof(EnumSwitchExpressionArmAttribute);

	/// <summary>
	/// Indicates the attribute name of the enum switch expression root.
	/// </summary>
	public const string EnumSwitchExpressionRootAttribute = nameof(EnumSwitchExpressionRootAttribute);

	/// <summary>
	/// Indicates the type name of the enum switch expression default behavior.
	/// </summary>
	public const string EnumSwitchExpressionDefaultBehavior = nameof(EnumSwitchExpressionDefaultBehavior);
}
