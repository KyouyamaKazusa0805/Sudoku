namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents links the current technique is linked to
/// <see href="http://forum.enjoysudoku.com/collection-of-solving-techniques-t3315.html">EnjoySudoku forum</see>
/// and <see href="http://sudopedia.enjoysudoku.com/Solving_Technique.html">its mirror site</see>.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed partial class ReferenceLinkAttribute([RecordParameter, StringSyntax(StringSyntax.Uri)] string link) : Attribute;
