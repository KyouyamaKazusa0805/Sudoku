using System;

namespace Sudoku.CodeGenerating
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
		/// ID.
		/// </summary>
		/// <param name="supportedDiagnosticId">The ID of all supported diagnostic result.</param>
		public CodeFixProviderAttribute(string supportedDiagnosticId) => SupportedDiagnosticId = supportedDiagnosticId;


		/// <summary>
		/// Indicates the supported diagnostic ID.
		/// </summary>
		public string SupportedDiagnosticId { get; }
	}
}
