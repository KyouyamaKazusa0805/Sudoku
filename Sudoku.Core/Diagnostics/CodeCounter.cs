using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sudoku.Extensions;

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
		private readonly string? _pattern;


		/// <summary>
		/// Initializes an instance with the specified root directory.
		/// </summary>
		/// <param name="root">The directory.</param>
		public CodeCounter(string root) : this(root, null)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified root directory and the filter pattern.
		/// </summary>
		/// <param name="root">The root directory.</param>
		/// <param name="filterPattern">
		/// The filter pattern.
		/// </param>
		/// <example>
		/// For example, you can write code like this:
		/// <code>
		/// var cc = new CodeCounter(<br/>
		///     root: @"C:\Users\HelloWorld\Desktop\Project",<br/>
		///     filterPattern: @".+\.cs$");
		/// </code>
		/// </example>
		public CodeCounter(string root, string? filterPattern) => (_root, _pattern) = (root, filterPattern);


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
			GetAllFilesRecursively(new(_root));

			int count = 0;
			int result = 0;
			foreach (string fileName in FileList)
			{
				try
				{
					using var sr = new StreamReader(fileName);
					int temp = 0;
					while (!(sr.ReadLine() is null))
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
				where _pattern is null || File.FullName.SatisfyPattern(_pattern)
				select File.FullName);

			// Get all files for each folder recursively.
			foreach (var d in directory.GetDirectories())
			{
				GetAllFilesRecursively(d);
			}
		}
	}
}
