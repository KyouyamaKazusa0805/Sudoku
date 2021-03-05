using System;
using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sudoku.Diagnostics
{
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
	public sealed record FileCounterResult(
		int ResultLines, int FilesCount, long CharactersCount, long Bytes,
		in TimeSpan Elapsed, IList<string> FileList)
	{
		/// <inheritdoc/>
		public override string ToString() =>
			new StringBuilder()
			.AppendLine("Results:")
			.Append("* Code lines: ")
			.AppendLine(ResultLines)
			.Append("* Files: ")
			.AppendLine(FilesCount)
			.Append("* Characters: ")
			.AppendLine(CharactersCount)
			.Append("* Bytes: ")
			.Append(SizeUnitConverter.Convert(Bytes, out var unit).ToString(".000"))
			.Append(' ')
			.Append(ToString(unit))
			.Append(" (")
			.Append(Bytes)
			.AppendLine(" Bytes)")
			.Append("* Time elapsed: ")
			.AppendLine(Elapsed.ToString("hh\\:mm\\.ss\\.fff"))
			.Append("About more information, please call each property in this instance.")
			.ToString();


		/// <summary>
		/// Gets the specified string notation for a <see cref="SizeUnit"/> instance.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <returns>The string result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string ToString(SizeUnit @this) =>
			@this switch
			{
				SizeUnit.Byte => "B",
				SizeUnit.Kilobyte => "KB",
				SizeUnit.Megabyte => "MB",
				SizeUnit.Gigabyte => "GB",
				SizeUnit.Terabyte => "TB"
			};
	}
}
