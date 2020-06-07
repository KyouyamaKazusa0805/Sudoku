using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>exocet</b> technique.
	/// </summary>
	public abstract class ExocetTechniqueInfo : TechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="exocet">The exocet.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="techniqueCode">The technique code.</param>
		/// <param name="lockedMemberQ">The locked member Q.</param>
		/// <param name="lockedMemberR">The locked member R.</param>
		/// <param name="targetEliminations">The target eliminations.</param>
		/// <param name="mirrorEliminations">The mirror eliminations.</param>
		/// <param name="bibiEliminations">
		/// The Bi-bi pattern eliminations (only used for junior exocets).
		/// </param>
		/// <param name="targetPairEliminations">
		/// The target pair eliminations (only used for junior exocets).
		/// </param>
		/// <param name="swordfishEliminations">
		/// The swordfish pattern eliminations (only used for junior exocets).
		/// </param>
		/// <param name="trueBaseEliminations">
		/// The true base eliminations (only used for senior exocets).
		/// </param>
		/// <param name="compatibilityEliminations">
		/// The compatibility test eliminations (only used for senior exocets).
		/// </param>
		public ExocetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			Exocet exocet, IEnumerable<int> digits, TechniqueCode techniqueCode,
			IEnumerable<int>? lockedMemberQ, IEnumerable<int>? lockedMemberR,
			TargetEliminations targetEliminations, MirrorEliminations mirrorEliminations,
			BibiPatternEliminations bibiEliminations, TargetPairEliminations targetPairEliminations,
			SwordfishEliminations swordfishEliminations, TrueBaseEliminations trueBaseEliminations,
			CompatibilityTestEliminations compatibilityEliminations)
			: base(conclusions, views)
		{
			(Exocet, Digits, TechniqueCode, LockedMemberQ, LockedMemberR) = (exocet, digits, techniqueCode, lockedMemberQ, lockedMemberR);

			var list = (List<Conclusion>)Conclusions;
			if (!((TargetEliminations = targetEliminations).Conclusions is null))
			{
				list.AddRange(TargetEliminations);
			}
			if (!((MirrorEliminations = mirrorEliminations).Conclusions is null))
			{
				list.AddRange(MirrorEliminations);
			}
			if (!((BibiEliminations = bibiEliminations).Conclusions is null))
			{
				list.AddRange(BibiEliminations);
			}
			if (!((TargetPairEliminations = targetPairEliminations).Conclusions is null))
			{
				list.AddRange(TargetPairEliminations);
			}
			if (!((SwordfishEliminations = swordfishEliminations).Conclusions is null))
			{
				list.AddRange(SwordfishEliminations);
			}
			if (!((TrueBaseEliminations = trueBaseEliminations).Conclusions is null))
			{
				list.AddRange(TrueBaseEliminations);
			}
			if (!((CompatibilityTestEliminations = compatibilityEliminations).Conclusions is null))
			{
				list.AddRange(CompatibilityTestEliminations);
			}

			var temp = Conclusions.Distinct().ToList(); // Call 'ToList' to execute the query forcedly.
			list.Clear();
			list.AddRange(temp);
		}


		/// <summary>
		/// The locked member Q.
		/// </summary>
		public IEnumerable<int>? LockedMemberQ { get; }

		/// <summary>
		/// The locked member R.
		/// </summary>
		public IEnumerable<int>? LockedMemberR { get; }

		/// <summary>
		/// Indicates all digits used.
		/// </summary>
		public IEnumerable<int> Digits { get; }

		/// <summary>
		/// The exocet.
		/// </summary>
		public Exocet Exocet { get; }

		/// <summary>
		/// The target eliminations.
		/// </summary>
		public TargetEliminations TargetEliminations { get; }

		/// <summary>
		/// The mirror eliminations.
		/// </summary>
		public MirrorEliminations MirrorEliminations { get; }

		/// <summary>
		/// The Bi-bi pattern eliminations.
		/// </summary>
		public BibiPatternEliminations BibiEliminations { get; }

		/// <summary>
		/// The target pair eliminations.
		/// </summary>
		public TargetPairEliminations TargetPairEliminations { get; }

		/// <summary>
		/// The swordfish eliminations.
		/// </summary>
		public SwordfishEliminations SwordfishEliminations { get; }

		/// <summary>
		/// The true base eliminations.
		/// </summary>
		public TrueBaseEliminations TrueBaseEliminations { get; }

		/// <summary>
		/// The compatibility test eliminations.
		/// </summary>
		public CompatibilityTestEliminations CompatibilityTestEliminations { get; }

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

		/// <inheritdoc/>
		public sealed override TechniqueCode TechniqueCode { get; }



		/// <inheritdoc/>
		public override string ToString()
		{
			var (b1, b2, tq1, tq2, tr1, tr2) = Exocet;
			string? addtional = GetAdditional();

			return new StringBuilder(Name)
				.Append(": Digits ")
				.Append(new DigitCollection(Digits).ToString())
				.Append(" in base cells ")
				.Append(new CellCollection(stackalloc[] { b1, b2 }).ToString())
				.Append(", target cells ")
				.Append(new CellCollection(stackalloc[] { tq1, tq2, tr1, tr2 }).ToString())
				.NullableAppend(
					LockedMemberQ is null ? null : $", locked member 1: {new DigitCollection(LockedMemberQ).ToString()}")
				.NullableAppend(
					LockedMemberR is null ? null : $", locked member 2: {new DigitCollection(LockedMemberR).ToString()}")
				.Append(addtional is null ? string.Empty : $" with {addtional}")
				.Append(" => ")
				.AppendLine(new ConclusionCollection(Conclusions).ToString())
				.NullableAppendLine(TargetEliminations.ToString())
				.NullableAppendLine(MirrorEliminations.ToString())
				.NullableAppendLine(BibiEliminations.ToString())
				.NullableAppendLine(TargetPairEliminations.ToString())
				.NullableAppendLine(SwordfishEliminations.ToString())
				.NullableAppendLine(TrueBaseEliminations.ToString())
				.NullableAppendLine(CompatibilityTestEliminations.ToString())
				.ToString();
		}

		/// <summary>
		/// Get the additional message.
		/// </summary>
		/// <returns>The additional message.</returns>
		protected abstract string? GetAdditional();
	}
}
