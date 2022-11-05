namespace Sudoku.Text.Formatting;

/// <summary>
/// Indicates the extended format.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ExtendedFormatAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="ExtendedFormatAttribute"/> instance via the format.
	/// </summary>
	/// <param name="format">The format.</param>
	public ExtendedFormatAttribute(string format) => Format = format;


	/// <summary>
	/// Indicates the extended format.
	/// </summary>
	public string Format { get; }
}
