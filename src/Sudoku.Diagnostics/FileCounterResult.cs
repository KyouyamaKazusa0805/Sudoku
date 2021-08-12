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
		public override string ToString()
		{
			var sb = new ValueStringBuilder(300);
			sb.AppendLine("Results:");
			sb.Append("* Code lines: ");
			sb.AppendLine(ResultLines);
			sb.Append("* Files: ");
			sb.AppendLine(FilesCount);
			sb.Append("* Characters: ");
			sb.AppendLine(CharactersCount);
			sb.Append("* Bytes: ");
			sb.Append(SizeUnitConverter.Convert(Bytes, out var unit).ToString(".000"));
			sb.Append(' ');
			sb.Append(ToString(unit));
			sb.Append(" (");
			sb.Append(Bytes);
			sb.AppendLine(" Bytes)");
			sb.Append("* Time elapsed: ");
			sb.AppendLine(Elapsed.ToString("hh\\:mm\\.ss\\.fff"));
			sb.Append("About more information, please call each property in this instance.");

			return sb.ToString();
		}


		/// <summary>
		/// Gets the specified string notation for a <see cref="SizeUnit"/> instance.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <returns>The string result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string ToString(SizeUnit @this) => @this switch
		{
			SizeUnit.Byte => "B",
			SizeUnit.Kilobyte => "KB",
			SizeUnit.Megabyte => "MB",
			SizeUnit.Gigabyte => "GB",
			SizeUnit.Terabyte => "TB"
		};
	}
}
