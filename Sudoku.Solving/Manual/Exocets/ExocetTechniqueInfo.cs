using System.Collections.Generic;
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
		/// <param name="mirrorEliminations">The mirror eliminations.</param>
		public ExocetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			Exocet exocet, IEnumerable<int> digits, ExocetTypeCode typeCode,
			MirrorEliminations mirrorEliminations)
			: base(conclusions, views)
		{
			(Exocet, Digits, TypeCode) = (exocet, digits, typeCode);
			if (!((MirrorEliminations = mirrorEliminations).Conclusions is null))
			{
				((List<Conclusion>)Conclusions).AddRange(MirrorEliminations);
			}
		}


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
		/// The mirror eliminations.
		/// </summary>
		public MirrorEliminations MirrorEliminations { get; }

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
				.Append(addtional is null ? string.Empty : $" with {addtional}")
				.Append(" => ")
				.Append(ConclusionCollection.ToString(Conclusions))
				.NullableAppendLine(MirrorEliminations.ToString())
				.ToString();
		}

		/// <summary>
		/// Get the additional message.
		/// </summary>
		/// <returns>The additional message.</returns>
		protected abstract string? GetAdditional();
	}
}
