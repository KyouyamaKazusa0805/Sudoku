namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Indicates the property generating data for XML documentation comments.
/// </summary>
/// <param name="DocSummary">Indicates the documentation comment <c>summary</c> part.</param>
/// <param name="DocRemarks">Indicates the documentation comment <c>remarks</c> part.</param>
/// <param name="DocCref">Indicates the referenced member name that will be used for displaying <c>inheritdoc</c> part.</param>
/// <param name="DocPath">Indicates the referenced path that will be used for displaying <c>inheritdoc</c> part.</param>
internal readonly record struct DocumentationCommentData(string? DocSummary, string? DocRemarks, string? DocCref, string? DocPath);
