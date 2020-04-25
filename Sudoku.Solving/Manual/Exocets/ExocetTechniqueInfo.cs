using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>exocet</b> technique.
	/// </summary>
	public abstract class ExocetTechniqueInfo : TechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="exocet">The exocet.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="typeCode">The type code.</param>
		/// <param name="lockedMemberQ">The locked member Q.</param>
		/// <param name="lockedMemberR">The locked member R.</param>
		/// <param name="targetEliminations">The target eliminations.</param>
		/// <param name="mirrorEliminations">The mirror eliminations.</param>
		/// <param name="bibiEliminations">The Bi-bi pattern eliminations.</param>
		/// <param name="targetPairEliminations">The target pair eliminations.</param>
		/// <param name="swordfishEliminations">The swordfish pattern eliminations.</param>
		public ExocetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			Exocet exocet, IEnumerable<int> digits, ExocetTypeCode typeCode,
			IEnumerable<int>? lockedMemberQ, IEnumerable<int>? lockedMemberR,
			TargetEliminations targetEliminations, MirrorEliminations mirrorEliminations,
			BibiPatternEliminations bibiEliminations, TargetPairEliminations targetPairEliminations,
			SwordfishEliminations swordfishEliminations)
			: base(conclusions, views)
		{
			(Exocet, Digits, TypeCode, LockedMemberQ, LockedMemberR) = (exocet, digits, typeCode, lockedMemberQ, lockedMemberR);

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
		/// Indicates the type code of this exocet.
		/// </summary>
		public ExocetTypeCode TypeCode { get; }

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

		/// <inheritdoc/>
		public sealed override string Name => TypeCode.GetCustomName()!;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			var (b1, b2, tq1, tq2, tr1, tr2) = Exocet;
			string? addtional = GetAdditional();

			return new StringBuilder(Name)
				.Append(": Digits ")
				.Append(DigitCollection.ToString(Digits))
				.Append(" in base cells ")
				.Append(CellCollection.ToString(new[] { b1, b2 }))
				.Append(", target cells ")
				.Append(CellCollection.ToString(new[] { tq1, tq2, tr1, tr2 }))
				.NullableAppend(LockedMemberQ is null ? null : $", locked member 1: {DigitCollection.ToString(LockedMemberQ)}")
				.NullableAppend(LockedMemberR is null ? null : $", locked member 2: {DigitCollection.ToString(LockedMemberR)}")
				.Append(addtional is null ? string.Empty : $" with {addtional}")
				.Append(" => ")
				.AppendLine(ConclusionCollection.ToString(Conclusions))
				.NullableAppendLine(TargetEliminations.ToString())
				.NullableAppendLine(MirrorEliminations.ToString())
				.NullableAppendLine(BibiEliminations.ToString())
				.ToString();
		}

		/// <summary>
		/// Get the additional message.
		/// </summary>
		/// <returns>The additional message.</returns>
		protected abstract string? GetAdditional();
	}
}
