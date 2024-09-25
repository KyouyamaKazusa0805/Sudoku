namespace Sudoku.Runtime.InteropServices.Npg;

/// <summary>
/// Represents compatibility rules for Number-Place Generator.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class NpgAttribute : ProgramMetadataAttribute<int>;
