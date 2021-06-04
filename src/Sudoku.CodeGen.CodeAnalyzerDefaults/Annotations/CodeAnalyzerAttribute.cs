using System;

namespace Sudoku.CodeGen
{
	/// <summary>
	/// To mark on a code analyzer type, to tell the compiler and the source generator that
	/// the type should generate the default values.
	/// </summary>
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
