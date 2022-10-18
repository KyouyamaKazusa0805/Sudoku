namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// The special full type names.
/// </summary>
internal static class SpecialFullTypeNames
{
	public const string IsLargeStructAttribute = $"System.Diagnostics.CodeAnalysis.{nameof(IsLargeStructAttribute)}";
	public const string SelfAttribute = $"System.Diagnostics.CodeAnalysis.{nameof(SelfAttribute)}";
	public const string DisallowFunctionPointerInvocationAttribute = $"System.Diagnostics.CodeAnalysis.{nameof(DisallowFunctionPointerInvocationAttribute)}";
	public const string StringHandler = $"System.Text.{nameof(StringHandler)}";
	public const string FileAccessOnlyAttribute = $"System.Diagnostics.CodeAnalysis.{nameof(FileAccessOnlyAttribute)}";
}
