namespace Sudoku.Text.Formatting;

/// <summary>
/// Indicates the extended grid format.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ExtendedGridFormatAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="ExtendedGridFormatAttribute"/> instance.
	/// </summary>
	/// <param name="format">The format.</param>
	public ExtendedGridFormatAttribute(string format) => Format = format;


	/// <summary>
	/// Indicates the extended format.
	/// </summary>
	public string Format { get; }
}
