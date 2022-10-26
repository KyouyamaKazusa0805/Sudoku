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
	public const string Grid = $"Sudoku.Concepts.{nameof(Grid)}";
	public const string CellMap = $"Sudoku.Concepts.{nameof(CellMap)}";
	public const string SupportedArgumentsAttribute = $"System.CommandLine.Annotations.{nameof(SupportedArgumentsAttribute)}";
	public const string IExecutable = $"System.CommandLine.{nameof(IExecutable)}";
}
