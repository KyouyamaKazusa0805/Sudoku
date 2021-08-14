namespace Sudoku.Versioning;

/// <summary>
/// To mark onto a type (only for <see langword="class"/> or <see langword="struct"/>), to tell the user
/// and the compiler that the type is only used in algorithms-studying scenarios.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ForStudyingOnlyAttribute : Attribute
{
}
