using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Manual.Exocets.Eliminations;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>exocet</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Exocet">The exocet.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="TechniqueCode">The technique code.</param>
	/// <param name="LockedMemberQ">The locked member Q.</param>
	/// <param name="LockedMemberR">The locked member R.</param>
	/// <param name="TargetEliminations">The target eliminations.</param>
	/// <param name="MirrorEliminations">The mirror eliminations.</param>
	/// <param name="BibiEliminations">
	/// The Bi-bi pattern eliminations (only used for junior exocets).
	/// </param>
	/// <param name="TargetPairEliminations">
	/// The target pair eliminations (only used for junior exocets).
	/// </param>
	/// <param name="SwordfishEliminations">
	/// The swordfish pattern eliminations (only used for junior exocets).
	/// </param>
	/// <param name="TrueBaseEliminations">
	/// The true base eliminations (only used for senior exocets).
	/// </param>
	/// <param name="CompatibilityEliminations">
	/// The compatibility test eliminations (only used for senior exocets).
	/// </param>
	public abstract record ExocetTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		Pattern Exocet, IEnumerable<int> Digits, TechniqueCode TechniqueCode,
		IEnumerable<int>? LockedMemberQ, IEnumerable<int>? LockedMemberR,
		TargetEliminations TargetEliminations, MirrorEliminations MirrorEliminations,
		BibiPatternEliminations BibiEliminations, TargetPairEliminations TargetPairEliminations,
		SwordfishEliminations SwordfishEliminations, TrueBaseEliminations TrueBaseEliminations,
		CompatibilityTestEliminations CompatibilityEliminations)
		: TechniqueInfo(GatherConclusions(Conclusions, TargetEliminations, MirrorEliminations, BibiEliminations, TargetPairEliminations, SwordfishEliminations, TrueBaseEliminations, CompatibilityEliminations), Views)
	{
		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <inheritdoc/>
		public abstract override decimal Difficulty { get; }

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			var (baseMap, targetMap, _) = Exocet;
			string? addtional = GetAdditional();

			return new StringBuilder(Name)
				.Append(": Digits ")
				.Append(new DigitCollection(Digits).ToString())
				.Append(" in base cells ")
				.Append(new CellCollection(baseMap).ToString())
				.Append(", target cells ")
				.Append(new CellCollection(targetMap).ToString())
				.NullableAppend(
					LockedMemberQ is null ? null : $", locked member 1: {new DigitCollection(LockedMemberQ).ToString()}")
				.NullableAppend(
					LockedMemberR is null ? null : $", locked member 2: {new DigitCollection(LockedMemberR).ToString()}")
				.Append(addtional is null ? string.Empty : $" with {addtional}")
				.Append(" => ")
				.Append(new ConclusionCollection(Conclusions).ToString())
				.ToString();
		}

		/// <inheritdoc/>
		public sealed override string ToFullString() =>
			new StringBuilder(ToString())
				.NullableAppendLine(TargetEliminations.ToString())
				.NullableAppendLine(MirrorEliminations.ToString())
				.NullableAppendLine(BibiEliminations.ToString())
				.NullableAppendLine(TargetPairEliminations.ToString())
				.NullableAppendLine(SwordfishEliminations.ToString())
				.NullableAppendLine(TrueBaseEliminations.ToString())
				.NullableAppendLine(CompatibilityEliminations.ToString())
				.ToString();

		/// <summary>
		/// Get the additional message.
		/// </summary>
		/// <returns>The additional message.</returns>
		protected abstract string? GetAdditional();


		/// <summary>
		/// Gather conclusions.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="targetEliminations">The target eliminations.</param>
		/// <param name="mirrorEliminations">The mirror eliminations.</param>
		/// <param name="bibiEliminations">The Bi-bi pattern eliminations.</param>
		/// <param name="targetPairEliminations">The target pair eliminations.</param>
		/// <param name="swordfishEliminations">The swordfish eliminations.</param>
		/// <param name="trueBaseEliminations">The true base eliminations.</param>
		/// <param name="compatibilityEliminations">The compatibility eliminations.</param>
		/// <returns>The gathered result.</returns>
		private static IReadOnlyList<Conclusion> GatherConclusions(
			IReadOnlyList<Conclusion> conclusions,
			TargetEliminations targetEliminations, MirrorEliminations mirrorEliminations,
			BibiPatternEliminations bibiEliminations, TargetPairEliminations targetPairEliminations,
			SwordfishEliminations swordfishEliminations, TrueBaseEliminations trueBaseEliminations,
			CompatibilityTestEliminations compatibilityEliminations)
		{
			var list = (List<Conclusion>)conclusions;
			if (targetEliminations.Conclusions is not null)
			{
				list.AddRange(targetEliminations);
			}
			if (mirrorEliminations.Conclusions is not null)
			{
				list.AddRange(mirrorEliminations);
			}
			if (bibiEliminations.Conclusions is not null)
			{
				list.AddRange(bibiEliminations);
			}
			if (targetPairEliminations.Conclusions is not null)
			{
				list.AddRange(targetPairEliminations);
			}
			if (swordfishEliminations.Conclusions is not null)
			{
				list.AddRange(swordfishEliminations);
			}
			if (trueBaseEliminations.Conclusions is not null)
			{
				list.AddRange(trueBaseEliminations);
			}
			if (compatibilityEliminations.Conclusions is not null)
			{
				list.AddRange(compatibilityEliminations);
			}

			var temp = conclusions.Distinct().ToList(); // Call 'ToList' to execute the query forcedly.
			list.Clear();
			list.AddRange(temp);

			return list;
		}
	}
}
