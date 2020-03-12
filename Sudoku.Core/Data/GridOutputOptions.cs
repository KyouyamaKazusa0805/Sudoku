using System;

namespace Sudoku.Data
{
	/// <summary>
	/// Provides a grid output option.
	/// </summary>
	[Flags]
	public enum GridOutputOptions : byte
	{
		/// <summary>
		/// Indicates the default settings (a single-line string text).
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates the output should be with modifiable values.
		/// </summary>
		WithModifiers = 1,

		/// <summary>
		/// <para>
		/// Indicates the output should be with candidates.
		/// If the output is single line, the candidates will indicate
		/// the candidates-have-eliminated before the current grid status;
		/// if the output is multi-line, the candidates will indicate
		/// the real candidate at the current grid status.
		/// </para>
		/// <para>
		/// If the output is single line, the output will append the candidates
		/// value at the tail of the string in ':candidate list'. In addition,
		/// candidates will be represented as 'digit', 'row offset' and
		/// 'column offset' in order.
		/// </para>
		/// </summary>
		WithCandidates = 2,

		/// <summary>
		/// Indicates the output will treat modifiable values as given ones.
		/// If the output is single line, the output will remove all plus marks '+'.
		/// If the output is multi-line, the output will use '&lt;digit&gt;' instead
		/// of '*digit*'.
		/// </summary>
		TreatValueAsGiven = 4,

		/// <summary>
		/// Indicates whether need to handle all grid outlines while outputting.
		/// See file "How to use 'Grid' class.md" for more information.
		/// </summary>
		SubtleGridLines = 8,

		/// <summary>
		/// Indicates whether the output will be compatible with Hodoku library format.
		/// </summary>
		HodokuCompatible = 16,

		/// <summary>
		/// Indicates the placeholder must be '.' instead of '0'.
		/// If the value is <see langword="true"/>, the placeholder will be '.';
		/// otherwise, '0'.
		/// </summary>
		DotPlaceholder = 32,

		/// <summary>
		/// Indicates whether the output should be multi-line.
		/// </summary>
		Multiline = 64,
	}
}
