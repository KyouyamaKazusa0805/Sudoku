namespace Sudoku.UI.Drawing.Metadata;

/// <summary>
/// Indicates the specified shape kind is not supported in <see cref="CandidateMark"/>.
/// </summary>
/// <seealso cref="CandidateMark"/>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
internal sealed class NotSupportedCandidateMarkAttribute : Attribute
{
}
