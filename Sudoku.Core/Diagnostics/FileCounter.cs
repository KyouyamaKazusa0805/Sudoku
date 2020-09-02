#if CSHARP_9_PREVIEW
#pragma warning disable CS1591
#endif

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sudoku.Extensions;

namespace Sudoku.Diagnostics
{
	/// <summary>
	/// Encapsulates a file counter.
	/// </summary>
	public sealed record FileCounter(string Root, string? Pattern, bool WithBinOrObjDirectory, IList<string> FileList)
	{
		/// <summary>
		/// Indicates the comment lint regular expression instance.
		/// </summary>
		private static readonly Regex CommentLineRegex = new(@"(\s//.+|/\*.+|.+\*/)", RegexOptions.Compiled);


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
		public FileCounter(string root, string? extension) : this(root, $@".+\.{extension}$", true, new List<string>())
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
		/// Indicates the total number of the comment lines.
		/// </summary>
		public int CommentLines { get; private set; }

		/// <summary>
		/// Indicates the number of files.
		/// </summary>
		public int FilesCount { get; private set; }

		/// <summary>
		/// Indicates the number of result lines.
		/// </summary>
		public int ResultLines { get; private set; }

		/// <summary>
		/// Indicates the characters in files.
		/// </summary>
		public long CharactersCount { get; private set; }

		/// <summary>
		/// Indicates the length of all files.
		/// </summary>
		public long Bytes { get; private set; }


		/// <summary>
		/// Count on code lines in all files in the specified root directory.
		/// </summary>
		/// <returns>The number of lines.</returns>
		public void CountCodeLines()
		{
			g(new(Root));

			(FilesCount, CharactersCount, Bytes) = (0, 0, 0);
			foreach (string fileName in FileList)
			{
				try
				{
					using var sr = new StreamReader(fileName);
					int fileLines = 0;
					string? s;
					while ((s = sr.ReadLine()) is not null)
					{
						fileLines++;
						CharactersCount += s.Length;

						// Remove header \t.
						CharactersCount -= s.Reserve(@"\t").Length;

						// Check whether the current line is comment line.
						if (CommentLineRegex.Match(s) is Match { Success: true })
						{
							CommentLines++;
						}
					}

					ResultLines += fileLines;
					Bytes += new FileInfo(fileName).Length;
					FilesCount++;
				}
				catch
				{
					// Do nothing.
				}
			}

			void g(DirectoryInfo directory)
			{
				FileList.AddRange(
					from File in directory.GetFiles()
					where Pattern is null || File.FullName.SatisfyPattern(Pattern)
					select File.FullName);

				// Get all files for each folder recursively.
				foreach (var d in
					from Dir in directory.GetDirectories()
					let Name = Dir.Name
					where
						Name.Length > 0 && Name[0] is >= 'A' and <= 'Z'
						&& (
							!WithBinOrObjDirectory
							&& Name is not ("bin" or "Bin" or "obj" or "Obj")
							|| WithBinOrObjDirectory)
					select Dir)
				{
					g(d);
				}
			}
		}
	}
}
