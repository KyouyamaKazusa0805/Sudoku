namespace System.CommandLine;

/// <summary>
/// Represents a value converter.
/// </summary>
public interface IValueConverter
{
	/// <summary>
	/// Converts a string value as the command line argument into the target typed instance.
	/// </summary>
	/// <param name="value">The string value as the command line argument.</param>
	/// <returns>The target typed instance as the result.</returns>
	/// <exception cref="CommandConverterAttribute{TConverter}">
	/// Throws when the current method has encountered an unexpected error.
	/// </exception>
	public abstract object Convert(string value);
}
