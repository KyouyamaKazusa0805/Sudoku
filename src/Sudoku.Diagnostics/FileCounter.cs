namespace Sudoku.Diagnostics;

/// <summary>
/// Encapsulates a file counter.
/// </summary>
public sealed class FileCounter
{
	/// <summary>
	/// Initializes an instance with the specified root directory.
	/// </summary>
	/// <param name="root">The directory.</param>
	public FileCounter(string root) : this(root, null, true, new List<string>())
	{
	}

	/// <summary>
	/// Initializes an instance with the specified root directory,
	/// and the filter pattern. The pattern is specified as a file extension,
	/// such as <c>"cs"</c>.
	/// </summary>
	/// <param name="root">The root.</param>
	/// <param name="extension">
	/// The file extension. This parameter can be <see langword="null"/>. If
	/// so, the counter will sum up all files with all extensions.
	/// </param>
	public FileCounter(string root, string? extension)
		: this(root, $@".+\.{extension}$", true, new List<string>())
	{
	}

	/// <summary>
	/// Initializes an instance with the specified root directory,
	/// the file extension and a <see cref="bool"/> value indicating whether
	/// the counter will record the codes in directories <c>bin</c> and <c>obj</c>.
	/// </summary>
	/// <param name="root">The root.</param>
	/// <param name="extension">
	/// The file extension. This parameter can be <see langword="null"/>. If
	/// so, the counter will sum up all files with all extensions.
	/// </param>
	/// <param name="withBinOrObjDirectory">
	/// Indicates whether the counter will record the codes in directories
	/// <c>bin</c> and <c>obj</c>.
	/// </param>
	public FileCounter(string root, string? extension, bool withBinOrObjDirectory)
		: this(root, $@".+\.{extension}$", withBinOrObjDirectory, new List<string>())
	{
	}

	/// <summary>
	/// Initializes an instance with the specified root, extension, a <see cref="bool"/> value
	/// indicating whether the counter will searcher for bin or obj directory, and a file list.
	/// </summary>
	/// <param name="root">The root.</param>
	/// <param name="extension">The file extension.</param>
	/// <param name="withBinOrObjDirectory">
	/// A <see cref="bool"/> value indicating whether the counter will search for bin or obj directory.
	/// </param>
	/// <param name="fileList">A file list.</param>
	private FileCounter(string root, string? extension, bool withBinOrObjDirectory, IList<string> fileList)
	{
		Root = root;
		Pattern = $@".+\.{extension}$";
		WithBinOrObjDirectory = withBinOrObjDirectory;
		FileList = fileList;
	}


	/// <summary>
	/// The root directory.
	/// </summary>
	public string Root { get; }

	/// <summary>
	/// The pattern.
	/// </summary>
	public string? Pattern { get; }

	/// <summary>
	/// Indicates whether the searcher will find directories <c>bin</c> or <c>obj</c>.
	/// </summary>
	public bool WithBinOrObjDirectory { get; }

	/// <summary>
	/// The file list.
	/// </summary>
	public IList<string> FileList { get; } = new List<string>();


	/// <summary>
	/// Count up for all files in the specified root directory, and return the result.
	/// </summary>
	/// <returns>The result information.</returns>
	public FileCounterResult CountUp()
	{
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		g(new(Root));

		(int filesCount, int resultLines, long charactersCount, long bytes) = default((int, int, long, long));
		foreach (string fileName in FileList)
		{
			StreamReader? sr = null;
			try
			{
				sr = new(fileName);
				int fileLines = 0;
				string? s;
				while ((s = sr.ReadLine()) is not null)
				{
					fileLines++;
					charactersCount += s.Length;

					// Remove leading \t.
					charactersCount -= s.Reserve(RegularExpressions.Tab).Length;
				}

				resultLines += fileLines;
				bytes += new FileInfo(fileName).Length;
				filesCount++;
			}
			catch
			{
			}
			finally
			{
				sr?.Close();
			}
		}

		stopwatch.Stop();
		return new(resultLines, filesCount, charactersCount, bytes, stopwatch.Elapsed, FileList);

		void g(DirectoryInfo directory)
		{
			FileList.AddRange(
				from file in directory.GetFiles()
				select file.FullName into fullName
				where Pattern is null || fullName.SatisfyPattern(Pattern)
				select fullName
			);

			// Get all files for each folder recursively.
			foreach (var d in
				from dir in directory.GetDirectories()
				let name = dir.Name
				where name.Length > 0 && name[0] is >= 'A' and <= 'Z' && (
					!WithBinOrObjDirectory && name is not ("bin" or "Bin" or "obj" or "Obj")
					|| WithBinOrObjDirectory
				)
				select dir)
			{
				g(d);
			}
		}
	}

	/// <summary>
	/// Count up for all files in the specified root directory, and return the result asynchronously.
	/// </summary>
	/// <returns>The task of the operation.</returns>
	public async Task<FileCounterResult> CountUpAsync() => await Task.Run(CountUp);

	/// <summary>
	/// Count up for all files in the specified root directory, and return the result asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The task of the operation.</returns>
	public async Task<FileCounterResult> CountUpAsync(CancellationToken cancellationToken) =>
		await Task.Run(CountUp, cancellationToken);
}
