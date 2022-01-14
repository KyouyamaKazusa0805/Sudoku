namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines a caller that means the object calls the setter.
/// </summary>
public enum InitializationCaller : byte
{
	/// <summary>
	/// Indicates the caller is the source generator.
	/// </summary>
	SourceGenerator,

	/// <summary>
	/// Indicates the caller is the module initializer.
	/// </summary>
	ModuleInitializer
}