namespace Sudoku.Globalization
{
	/// <summary>
	/// Encapsulates a country code to tell different countries.
	/// </summary>
	/// <remarks>
	/// This <see langword="enum"/> is used for some formatted string output, i.e. a
	/// multi-language string.
	/// </remarks>
	public enum CountryCode : ushort
	{
		/// <summary>
		/// Indicates the language is none of them, which is the default value of this type.
		/// </summary>
		[Name("")]
		Default = 0,

		/// <summary>
		/// Indicates the American English.
		/// </summary>
		[Name("en-us")]
		EnUs = 1033,

		/// <summary>
		/// Indicates the Chinese (Mainland).
		/// </summary>
		[Name("zh-cn")]
		ZhCn = 2052,
	}
}
