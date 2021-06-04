using System;

namespace Sudoku.CodeGen
{
	/// <summary>
	/// To mark on a code fixer type, to tell the compiler and the source generator that
	/// generates the code for the code fix defaults.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CodeFixProviderAttribute : Attribute
	{
		/// <summary>
		/// Initializes a <see cref="CodeFixProviderAttribute"/> class instance using the specified
		/// ID list.
		/// </summary>
		/// <param name="supportedDiagnosticIds">The ID list of all supported diagnostic results.</param>
		public CodeFixProviderAttribute(params string[] supportedDiagnosticIds) =>
			SupportedDiagnosticIds = supportedDiagnosticIds;


		/// <summary>
		/// Indicates the supported diagnostic IDs.
		/// </summary>
		public string[] SupportedDiagnosticIds { get; }
	}
}
