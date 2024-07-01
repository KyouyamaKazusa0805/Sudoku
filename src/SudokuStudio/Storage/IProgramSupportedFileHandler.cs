namespace SudokuStudio.Storage;

/// <summary>
/// Provides with a handler that can handle a file type defined in type <see cref="FileExtensions"/>.
/// </summary>
/// <typeparam name="T">The type of the handled object.</typeparam>
/// <seealso cref="FileExtensions"/>
public interface IProgramSupportedFileHandler<T> where T : notnull, allows ref struct
{
	/// <summary>
	/// Indicates the supported file extension.
	/// </summary>
	public static abstract string SupportedFileExtension { get; }


	/// <summary>
	/// Reads and handles the file via its path, and returns the target parsed instance.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <returns>The parsed instance.</returns>
	public static abstract T? Read(string filePath);

	/// <summary>
	/// Writes the specified instance, and output the content into the target file path.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="instance">The instance to be written.</param>
	public static abstract void Write(string filePath, T instance);
}
