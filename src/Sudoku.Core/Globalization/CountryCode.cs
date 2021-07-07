using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Globalization
{
	/// <summary>
	/// Encapsulates a country code to tell different countries.
	/// </summary>
	/// <remarks>
	/// This <see langword="enum"/> is used for some formatted string output, i.e. a
	/// multi-language string.
	/// </remarks>
	[Closed]
	public enum CountryCode : short
	{
		/// <summary>
		/// Indicates the language is none of them, which is the default value of this type.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Indicates the American English.
		/// </summary>
		EnUs = 1033,

		/// <summary>
		/// Indicates the Chinese (Mainland).
		/// </summary>
		ZhCn = 2052,
	}
}
