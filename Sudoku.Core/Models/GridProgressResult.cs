using System;
using Sudoku.Windows;

namespace Sudoku.Models
{
	/// <summary>
	/// Encapsulates a progress result used for report the current state.
	/// </summary>
	public struct GridProgressResult : IEquatable<GridProgressResult>, IProgressResult
	{
		/// <summary>
		/// Initializes an instance with the specified current point and the total point.
		/// </summary>
		/// <param name="currentCandidatesCount">The current point.</param>
		/// <param name="currentCellsCount">The number of unsolved cells.</param>
		/// <param name="initialCandidatesCount">The number of unsolved candidates in the initial grid.</param>
		/// <param name="globalizationString">The globalization string.</param>
		public GridProgressResult(
			int currentCandidatesCount, int currentCellsCount, int initialCandidatesCount,
			string? globalizationString) =>
			(CurrentCandidatesCount, CurrentCellsCount, InitialCandidatesCount, GlobalizationString) =
			(currentCandidatesCount, currentCellsCount, initialCandidatesCount, globalizationString ?? "en-us");


		/// <summary>
		/// Indicates the number of unsolved cells.
		/// </summary>
		public int CurrentCellsCount { readonly get; set; }

		/// <summary>
		/// Indicates the number of unsolved candidates.
		/// </summary>
		public int CurrentCandidatesCount { readonly get; set; }

		/// <summary>
		/// Indicates the number of unsolved candidates in the initial grid.
		/// </summary>
		public readonly int InitialCandidatesCount { get; }

		/// <summary>
		/// The globalization string.
		/// </summary>
		public readonly string GlobalizationString { get; }

		/// <summary>
		/// Indicates the current percentage.
		/// </summary>
		public readonly double Percentage =>
			(double)(InitialCandidatesCount - CurrentCandidatesCount) / InitialCandidatesCount * 100;


		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="current">(<see langword="out"/> parameter) The number of unsolved candidates.</param>
		/// <param name="unsolvedCells">(<see langword="out"/> parameter) The number of unsolved cells.</param>
		public readonly void Deconstruct(out int current, out int unsolvedCells) =>
			(current, unsolvedCells) = (CurrentCandidatesCount, CurrentCellsCount);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="currentCandidatesCount">(<see langword="out"/> parameter) The number of unsolved candidates.</param>
		/// <param name="currentCellsCount">(<see langword="out"/> parameter) The number of unsolved cells.</param>
		/// <param name="initialCandidatesCount">
		/// (<see langword="out"/> parameter) The number of unsolved candidates in the initial grid.
		/// </param>
		public readonly void Deconstruct(
			out int currentCandidatesCount, out int currentCellsCount, out int initialCandidatesCount) =>
			(currentCandidatesCount, currentCellsCount, initialCandidatesCount) =
			(CurrentCandidatesCount, CurrentCellsCount, InitialCandidatesCount);

		/// <inheritdoc cref="object.ToString"/>
		public override readonly string ToString() =>
			$"{Resources.GetValue("UnsolvedCells")}{CurrentCellsCount}" +
			$"{Resources.GetValue("UnsolvedCandidates")}{CurrentCandidatesCount}";

		/// <inheritdoc cref="object.Equals(object?)"/>
		public override readonly bool Equals(object? obj) => obj is GridProgressResult comparer && Equals(comparer);

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public readonly bool Equals(GridProgressResult other) =>
			CurrentCellsCount == other.CurrentCellsCount && CurrentCandidatesCount == other.CurrentCandidatesCount
			&& InitialCandidatesCount == other.InitialCandidatesCount && GlobalizationString == other.GlobalizationString;

		/// <inheritdoc cref="object.GetHashCode"/>
		public override readonly int GetHashCode() =>
			CurrentCellsCount * 729 + CurrentCandidatesCount ^ InitialCandidatesCount ^ GlobalizationString.GetHashCode();


		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(GridProgressResult left, GridProgressResult right) => left.Equals(right);

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(GridProgressResult left, GridProgressResult right) => !(left == right);
	}
}
