using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
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
	public abstract record ExocetStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Pattern Exocet, IEnumerable<int> Digits,
		IEnumerable<int>? LockedMemberQ, IEnumerable<int>? LockedMemberR,
		Target? TargetEliminations, Mirror? MirrorEliminations,
		BiBiPattern? BibiEliminations, TargetPair? TargetPairEliminations,
		Swordfish? SwordfishEliminations, TrueBase? TrueBaseEliminations,
		CompatibilityTest? CompatibilityEliminations)
		: StepInfo(
			GatherConclusions(
				Conclusions, TargetEliminations, MirrorEliminations, BibiEliminations,
				TargetPairEliminations, SwordfishEliminations, TrueBaseEliminations,
				CompatibilityEliminations),
			Views)
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
		public override string ToString() => ToStringInternal();

		/// <inheritdoc/>
		public sealed override string ToFullString() =>
			new StringBuilder(ToString())
				.AppendLine()
				.NullableAppendLine(TargetEliminations?.ToString())
				.NullableAppendLine(MirrorEliminations?.ToString())
				.NullableAppendLine(BibiEliminations?.ToString())
				.NullableAppendLine(TargetPairEliminations?.ToString())
				.NullableAppendLine(SwordfishEliminations?.ToString())
				.NullableAppendLine(TrueBaseEliminations?.ToString())
				.NullableAppendLine(CompatibilityEliminations?.ToString())
				.ToString();

		/// <summary>
		/// Get the additional message.
		/// </summary>
		/// <returns>The additional message.</returns>
		protected abstract string? GetAdditional();

		/// <summary>
		/// Same as <see cref="ToString"/> but the implementation part.
		/// </summary>
		/// <returns>The result.</returns>
		protected string ToStringInternal()
		{
			var (baseMap, targetMap, _) = Exocet;
			string? addtional = GetAdditional();

			return new StringBuilder(Name)
				.Append(": Digits ")
				.Append(new DigitCollection(Digits).ToString())
				.Append(" in base cells ")
				.Append(baseMap)
				.Append(", target cells ")
				.Append(targetMap)
				.NullableAppend(
					LockedMemberQ is null
					? null
					: $", locked member 1: {new DigitCollection(LockedMemberQ).ToString()}")
				.NullableAppend(
					LockedMemberR is null
					? null
					: $", locked member 2: {new DigitCollection(LockedMemberR).ToString()}")
				.Append(addtional is null ? string.Empty : $" with {addtional}")
				.Append(" => ")
				.Append(new ConclusionCollection(Conclusions).ToString())
				.ToString();
		}


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
			Target? targetEliminations, Mirror? mirrorEliminations,
			BiBiPattern? bibiEliminations, TargetPair? targetPairEliminations,
			Swordfish? swordfishEliminations, TrueBase? trueBaseEliminations,
			CompatibilityTest? compatibilityEliminations)
		{
			var list = (List<Conclusion>)conclusions;
			if (targetEliminations is not null)
			{
				list.AddRange(targetEliminations);
			}
			if (mirrorEliminations is not null)
			{
				list.AddRange(mirrorEliminations);
			}
			if (bibiEliminations is not null)
			{
				list.AddRange(bibiEliminations);
			}
			if (targetPairEliminations is not null)
			{
				list.AddRange(targetPairEliminations);
			}
			if (swordfishEliminations is not null)
			{
				list.AddRange(swordfishEliminations);
			}
			if (trueBaseEliminations is not null)
			{
				list.AddRange(trueBaseEliminations);
			}
			if (compatibilityEliminations is not null)
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
