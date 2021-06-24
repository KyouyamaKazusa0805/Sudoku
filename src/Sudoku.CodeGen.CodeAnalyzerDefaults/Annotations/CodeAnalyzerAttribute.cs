using System;

namespace Sudoku.CodeGen
{
	/// <summary>
	/// To mark on a code analyzer type, to tell the compiler and the source generator that
	/// the type should generate the default values.
	/// </summary>
	/// <remarks>
	/// The supported diagnostic IDs should satisfy the follow condition:
	/// <list type="number">
	/// <item>The ID should be combined with 2 upper-case letters and 4 numbers.</item>
	/// <item>The upper-case letters should be <c>SS</c> or <c>SD</c>.</item>
	/// <item>
	/// If the diagnostic result should fade out the code, just apply the suffix <c>"F"</c>, such as
	/// <c>SS0101F</c>, where the suffix <c>"F"</c> is the abbreviation of the phrase "fade out".
	/// </item>
	/// </list>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CodeAnalyzerAttribute : Attribute
	{
		/// <summary>
		/// Initializes a <see cref="CodeAnalyzerAttribute"/> class instance using the specified
		/// ID list.
		/// </summary>
		/// <param name="supportedDiagnosticIds">The ID list of all supported diagnostic results.</param>
		public CodeAnalyzerAttribute(params string[] supportedDiagnosticIds) =>
			SupportedDiagnosticIds = supportedDiagnosticIds;


		/// <summary>
		/// Indicates the supported diagnostic IDs.
		/// </summary>
		public string[] SupportedDiagnosticIds { get; }
	}
}
