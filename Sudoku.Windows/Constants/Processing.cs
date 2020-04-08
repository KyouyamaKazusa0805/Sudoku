using System;
using System.Reflection;
using Sudoku.Data.Extensions;

namespace Sudoku.Windows.Constants
{
	/// <summary>
	/// Provides all <see langword="const"/> or <see langword="readonly"/> values
	/// for internal processing.
	/// </summary>
	internal static class Processing
	{
		/// <summary>
		/// The splitter in using <see cref="string.Split(char[]?)"/> or other split methods
		/// where the method contain <see cref="char"/>[]? type parameter.
		/// </summary>
		/// <seealso cref="string.Split(char[]?)"/>
		public static readonly char[] Splitter = new[] { '\r', '\n' };


		/// <summary>
		/// Gets a new-line string defined for this environment.
		/// </summary>
		public static string NewLine => Environment.NewLine;

		/// <summary>
		/// Indicates the name of this solution.
		/// </summary>
		public static string SolutionName
		{
			get
			{
				return Assembly
					.GetExecutingAssembly()
					.GetCustomAttribute<AssemblyProductAttribute>()
					is AssemblyProductAttribute attr
					? attr.Product
					: "Sunnie's Sudoku Solution";
			}
		}

		/// <summary>
		/// Indicates the version.
		/// </summary>
		public static string SolutionVersion
		{
			get
			{
				return Assembly
					.GetExecutingAssembly()
					.GetName()
					.Version
					.NullableToString("Unknown version");
			}
		}
	}
}
