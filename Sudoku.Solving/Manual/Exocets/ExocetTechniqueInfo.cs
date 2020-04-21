using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>exocet</b> technique.
	/// </summary>
	public sealed class ExocetTechniqueInfo : TechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="exocet">The exocet.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="isSenior">Indicates whether the structure is senior.</param>
		public ExocetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			Exocet exocet, IEnumerable<int> digits, bool isSenior)
			: base(conclusions, views) =>
			(Exocet, IsSenior, Digits) = (exocet, isSenior, digits);


		/// <summary>
		/// Indicates all digits used.
		/// </summary>
		public IEnumerable<int> Digits { get; }

		/// <summary>
		/// Indicates the specified exocet is senior.
		/// </summary>
		public bool IsSenior { get; }

		/// <summary>
		/// The exocet.
		/// </summary>
		public Exocet Exocet { get; }

		/// <inheritdoc/>
		public override string Name => $"{(IsSenior ? "Senior" : "Junior")} Exocet";

		/// <inheritdoc/>
		public override decimal Difficulty => 9.4M + (IsSenior ? .2M : 0);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {Exocet} for digits {digitsStr} => {elimStr}";
		}
	}
}
