namespace Sudoku.Data
{
	partial struct GridMap
	{
		/// <summary>
		/// Provides an option for initialization of the base type <see cref="GridMap"/>.
		/// </summary>
		public enum InitializeOption : byte
		{
			/// <summary>
			/// Indicates each candidate will be processed with the normal case.
			/// </summary>
			Ordinary,

			/// <summary>
			/// Indicates each candidate will be processed with its peer cells.
			/// For example, if set the cell of the index 0, this option
			/// will let the constructor set its peer cells also.
			/// </summary>
			ProcessPeersAlso,

			/// <summary>
			/// Indicates each candidate will be processed with its peer cells,
			/// but itself will be set <see langword="false"/> rather than
			/// <see langword="true"/> value.
			/// </summary>
			ProcessPeersWithoutItself,
		}
	}
}
