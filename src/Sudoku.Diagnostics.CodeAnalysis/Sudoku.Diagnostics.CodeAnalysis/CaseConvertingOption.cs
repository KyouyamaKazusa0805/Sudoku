using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Defines an option that uses and specifies the case converting, which is used in
	/// <see cref="StringEx.ToCamelCase(string, CaseConvertingOption)"/>
	/// or <see cref="StringEx.ToPascalCase(string)"/>.
	/// </summary>
	/// <seealso cref="StringEx.ToCamelCase(string, CaseConvertingOption)"/>
	/// <seealso cref="StringEx.ToPascalCase(string)"/>
	public enum CaseConvertingOption
	{
		/// <summary>
		/// Indicates the option is none.
		/// </summary>
		None,

		/// <summary>
		/// Indicates the option will specify that the leading underscore in the identifier will be reserved
		/// after conversion from a case to another case, e.g. Camel case to pascal case.
		/// </summary>
		ReserveLeadingUnderscore
	}
}
