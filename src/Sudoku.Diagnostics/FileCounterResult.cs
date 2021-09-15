namespace Sudoku.Diagnostics;

/// <summary>
/// Encapsulates a result after <see cref="FileCounter"/>.
/// </summary>
/// <param name="ResultLines">The number of lines found.</param>
/// <param name="FilesCount">The number of files found.</param>
/// <param name="CharactersCount">The number of characters found.</param>
/// <param name="Bytes">All bytes.</param>
/// <param name="Elapsed">The elapsed time during searching.</param>
/// <param name="FileList">
/// The list of files. This property won't be output. If you want to use this property,
/// please write this property explicitly.
/// </param>
/// <seealso cref="FileCounter"/>
public sealed record class FileCounterResult(
	int ResultLines,
	int FilesCount,
	long CharactersCount,
	long Bytes,
	in TimeSpan Elapsed,
	IList<string> FileList
)
{
	/// <inheritdoc/>
	public override string ToString() =>
		$@"Results:
* Code lines: {ResultLines}
* Files: {FilesCount}
* Characters: {CharactersCount}
* Bytes: {SizeUnitConverter.Convert(Bytes, out var unit):.000} {unit switch
		{
			SizeUnit.Byte => "B",
			SizeUnit.Kilobyte => "KB",
			SizeUnit.Megabyte => "MB",
			SizeUnit.Gigabyte => "GB",
			SizeUnit.Terabyte => "TB"
		}} ({Bytes} Bytes)
* Time elapsed: {Elapsed:hh\:mm\.ss\.fff}
About more information, please call each property in this instance.";
}
