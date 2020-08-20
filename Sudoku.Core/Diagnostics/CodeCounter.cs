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
	/// Encapsulates a code line counter.
	/// </summary>
	public sealed record CodeLineCounter(string Root, string? Pattern)
	{
		/// <summary>
		/// Initializes an instance with the specified root directory.
		/// </summary>
		/// <param name="root">The directory.</param>
		public CodeLineCounter(string root) : this(root, null)
		{
		}


		/// <summary>
		/// All file paths at this root directory.
		/// </summary>
		public IList<string> FileList { get; } = new List<string>();


		/// <summary>
		/// Count on code lines in all files in the specified root directory.
		/// </summary>
		/// <param name="filesCount">
		/// (<see langword="out"/> parameter) The number of files searched.
		/// </param>
		/// <returns>The number of lines.</returns>
		public int CountCodeLines(out int filesCount)
		{
			GetAllFilesRecursively(new(Root));

			int count = 0;
			int result = 0;
			foreach (string fileName in FileList)
			{
				try
				{
					using var sr = new StreamReader(fileName);
					int temp = 0;
					while (sr.ReadLine() is not null)
					{
						temp++;
					}

					result += temp;
					count++;
				}
				catch { }
			}

			filesCount = count;
			return result;
		}

		/// <summary>
		/// Get all files in the specified directory recursively.
		/// </summary>
		/// <param name="directory">The directory information instance.</param>
		private void GetAllFilesRecursively(DirectoryInfo directory)
		{
			FileList.AddRange(
				from File in directory.GetFiles()
				where Pattern is null || File.FullName.SatisfyPattern(Pattern)
				select File.FullName);

			// Get all files for each folder recursively.
			foreach (var d in directory.GetDirectories())
			{
				GetAllFilesRecursively(d);
			}
		}
	}
}
