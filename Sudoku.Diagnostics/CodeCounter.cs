using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.Diagnostics
{
	/// <summary>
	/// Encapsulates a code line counter.
	/// </summary>
	public sealed class CodeCounter
	{
		/// <summary>
		/// The root directory of the project.
		/// </summary>
		private readonly string _root;

		/// <summary>
		/// The filter pattern.
		/// </summary>
		private readonly string _pattern;

		/// <summary>
		/// All file paths at this root directory.
		/// </summary>
		private readonly IList<string> _fileList = new List<string>();


		/// <summary>
		/// Initializes an instance with the specified root directory.
		/// </summary>
		/// <param name="root">The directory.</param>
		public CodeCounter(string root) : this(root, @".*\.txt$")
		{
		}

		/// <summary>
		/// Initializes an instance with the specified root directory and the filter pattern.
		/// </summary>
		/// <param name="root">The root directory.</param>
		/// <param name="filterPattern">The filter pattern.</param>
		[SuppressMessage("", "regex001")]
		public CodeCounter(string root, [Pattern] string filterPattern) =>
			(_root, _pattern) = (root, filterPattern);


		/// <summary>
		/// Count on code lines in all files in the specified root directory.
		/// </summary>
		/// <returns>The number of lines.</returns>
		public int CountCodeLines()
		{
			GetAllFilesRecursively(new DirectoryInfo(_root));

			int count = 0;
			foreach (string fileName in from fileName in _fileList
										where SatisfyPattern(fileName, _pattern)
										select fileName)
			{
				try
				{
					using var sr = new StreamReader(fileName);
					int temp = 0;
					while (!(sr.ReadLine() is null))
					{
						temp++;
					}

					count += temp;
				}
				catch
				{
				}
			}

			return count;
		}

		/// <summary>
		/// Check the string is whether satisfied the specified pattern strictly.
		/// </summary>
		/// <param name="str">The string to check.</param>
		/// <param name="pattern">The regular expression pattern.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		private static bool SatisfyPattern(string str, string pattern)
		{
			try
			{
				var match = Regex.Match(str, pattern);
				return match.Success ? match.Value == str : false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Get all files in the specified directory recursively.
		/// </summary>
		/// <param name="directory">The directory information instance.</param>
		private void GetAllFilesRecursively(DirectoryInfo directory)
		{
			var allFiles = directory.GetFiles();
			foreach (var fileInfo in allFiles)
			{
				_fileList.Add(fileInfo.FullName);
			}
			var allDirectories = directory.GetDirectories();
			foreach (var d in allDirectories)
			{
				GetAllFilesRecursively(d);
			}
		}
	}
}
