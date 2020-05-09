namespace Sudoku.Data
{
	partial class GridFormatter
	{
		/// <summary>
		/// The place holder.
		/// </summary>
		public char Placeholder { get; set; } = '.';

		/// <summary>
		/// Indicates the output should be with modifiable values.
		/// </summary>
		public bool WithModifiables { get; set; }

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
		public bool WithCandidates { get; set; }

		/// <summary>
		/// Indicates the output will treat modifiable values as given ones.
		/// If the output is single line, the output will remove all plus marks '+'.
		/// If the output is multi-line, the output will use '<c>&lt;digit&gt;</c>' instead
		/// of '<c>*digit*</c>'.
		/// </summary>
		public bool TreatValueAsGiven { get; set; }

		/// <summary>
		/// Indicates whether need to handle all grid outlines while outputting.
		/// See file "How to use 'Grid' class.md" for more information.
		/// </summary>
		public bool SubtleGridLines { get; set; }

		/// <summary>
		/// Indicates whether the output will be compatible with Hodoku library format.
		/// </summary>
		public bool HodokuCompatible { get; set; }

		/// <summary>
		/// Indicates the output will be sukaku format (all single-valued digit will
		/// be all treated as candidates).
		/// </summary>
		public bool Sukaku { get; set; }

		/// <summary>
		/// Indicates the output will be Excel format.
		/// </summary>
		public bool Excel { get; set; }
	}
}
