#if CSHARP_9_PREVIEW
#pragma warning disable CS1591
#endif

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sudoku.Extensions;

namespace Sudoku.Diagnostics
{
	/// <summary>
	/// Encapsulates a file counter.
	/// </summary>
	public sealed record FileCounter(string Root, string? Pattern, bool WithBinOrObjDirectory, IList<string> FileList)
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
		/// such as <c>"cs"</c> (without dot) or <c>".cs"</c> (with dot).
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
		/// Count on code lines in all files in the specified root directory.
		/// </summary>
		/// <param name="filesCount">
		/// (<see langword="out"/> parameter) The number of files searched.
		/// </param>
		/// <param name="charactersCount">
		/// (<see langword="out"/> parameter) The total characters from all files.
		/// </param>
		/// <param name="bytes">
		/// (<see langword="out"/> parameter) The total bytes from all files.
		/// </param>
		/// <returns>The number of lines.</returns>
		public int CountCodeLines(out int filesCount, out long charactersCount, out long bytes)
		{
			g(this, new(Root));

			(filesCount, charactersCount, bytes) = (0, 0, 0);
			int resultLines = 0;
			foreach (string fileName in FileList)
			{
				try
				{
					using var sr = new StreamReader(fileName);
					int fileLines = 0;
					string? tempStr;
					while ((tempStr = sr.ReadLine()) is not null)
					{
						fileLines++;
						charactersCount += tempStr?.Length ?? 0;

						// Remove trailing \t.
						charactersCount -= tempStr?.Reserve(@"\t").Length ?? 0;
					}

					resultLines += fileLines;
					bytes += new FileInfo(fileName).Length;
					filesCount++;
				}
				catch
				{
					// Do nothing.
				}
			}

			return resultLines;

			static void g(FileCounter @this, DirectoryInfo directory)
			{
				@this.FileList.AddRange(
					from File in directory.GetFiles()
					where @this.Pattern is null || File.FullName.SatisfyPattern(@this.Pattern)
					select File.FullName);

				// Get all files for each folder recursively.
				foreach (var d in
					from Dir in directory.GetDirectories()
					let Name = Dir.Name
					let Precondition = Name.Length > 0 && !Name.StartsWith('.') && Name[0] is >= 'A' and <= 'Z'
					let InnerCondition = !@this.WithBinOrObjDirectory && Name is not ("bin" or "Bin" or "obj" or "Obj") || @this.WithBinOrObjDirectory
					where Precondition && InnerCondition
					select Dir)
				{
					g(@this, d);
				}
			}
		}
	}
}
