namespace Sudoku.Models
{
	/// <summary>
	/// Encapsulates a progress result used for report the current state.
	/// </summary>
	[AutoDeconstruct(nameof(CurrentCandidatesCount), nameof(CurrentCellsCount))]
	[AutoDeconstruct(nameof(CurrentCandidatesCount), nameof(CurrentCellsCount), nameof(InitialCandidatesCount))]
	[AutoHashCode(nameof(BaseHashCode), nameof(InitialCandidatesCount), nameof(CountryCode))]
	[AutoEquality(nameof(CurrentCandidatesCount), nameof(CurrentCellsCount), nameof(InitialCandidatesCount), nameof(CountryCode))]
	public partial struct GridProgressResult : IValueEquatable<GridProgressResult>, IProgressResult
	{
		/// <summary>
		/// Initializes an instance with the specified current point and the total point.
		/// </summary>
		/// <param name="currentCandidatesCount">The current point.</param>
		/// <param name="currentCellsCount">The number of unsolved cells.</param>
		/// <param name="initialCandidatesCount">The number of unsolved candidates in the initial grid.</param>
		/// <param name="countryCode">The country code.</param>
		public GridProgressResult(
			int currentCandidatesCount, int currentCellsCount, int initialCandidatesCount,
			CountryCode countryCode)
		{
			CurrentCandidatesCount = currentCandidatesCount;
			CurrentCellsCount = currentCellsCount;
			InitialCandidatesCount = initialCandidatesCount;
			CountryCode = countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode;
		}


		/// <summary>
		/// Indicates the number of unsolved cells.
		/// </summary>
		public int CurrentCellsCount { get; set; }

		/// <summary>
		/// Indicates the number of unsolved candidates.
		/// </summary>
		public int CurrentCandidatesCount { get; set; }

		/// <summary>
		/// Indicates the number of unsolved candidates in the initial grid.
		/// </summary>
		public int InitialCandidatesCount { get; }

		/// <summary>
		/// The country code.
		/// </summary>
		public CountryCode CountryCode { get; }

		/// <summary>
		/// Indicates the current percentage.
		/// </summary>
		public readonly double Percentage =>
			(double)(InitialCandidatesCount - CurrentCandidatesCount) / InitialCandidatesCount * 100;

		/// <summary>
		/// Indicates the base hash code.
		/// </summary>
		private readonly int BaseHashCode => CurrentCellsCount * 729 + CurrentCandidatesCount;


		/// <inheritdoc cref="object.ToString"/>
		public override readonly string ToString()
		{
			var sb = new ValueStringBuilder(stackalloc char[50]);
			sb.Append((string)TextResources.Current.UnsolvedCells);
			sb.Append(CurrentCellsCount.ToString());
			sb.Append((string)TextResources.Current.UnsolvedCandidates);
			sb.Append(CurrentCandidatesCount.ToString());

			return sb.ToString();
		}
	}
}
