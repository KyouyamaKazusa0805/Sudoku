namespace Sudoku.CodeGenerating;

/// <summary>
/// To mark on a code analyzer type, to tell the compiler and the source generator that
/// the type should generate the default values.
/// </summary>
/// <remarks>
/// The supported diagnostic IDs should satisfy the follow condition:
/// <list type="number">
/// <item>The ID should be combined with 2 upper-case letters and 4 numbers.</item>
/// <item>The upper-case letters should be <c>SD</c>.</item>
/// <item>
/// If the diagnostic result should fade out the code, just apply the suffix <c>"F"</c>, such as
/// <c>SD0101F</c>, where the suffix <c>"F"</c> is the abbreviation of the phrase "fade out".
/// </item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
[Conditional("SUPPORT_CODE_ANALYZER")]
public sealed class CodeAnalyzerAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="CodeAnalyzerAttribute"/> class instance
	/// using the specified diagnostic ID list.
	/// </summary>
	/// <param name="supportedDiagnosticIdList">The supported diagnostic ID list.</param>
	public CodeAnalyzerAttribute(params string[] supportedDiagnosticIdList) =>
		SupportedDiagnosticIdList = supportedDiagnosticIdList;


	/// <summary>
	/// Indicates the supported diagnostic IDs.
	/// </summary>
	public string[] SupportedDiagnosticIdList { get; }
}
