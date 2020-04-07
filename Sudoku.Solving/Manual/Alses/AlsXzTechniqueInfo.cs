using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>ALS-XZ</b> technique.
	/// </summary>
	public sealed class AlsXzTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// The internal names.
		/// </summary>
		private static readonly string[] InternalNames =
		{
			null!, null!, "Generalized XYZ-Wing", "Generalized WXYZ-Wing", "Generalized VWXYZ-Wing",
			"Generalized UVWXYZ-Wing", "Generalized TUVWXYZ-Wing", "Generalized STUVWXYZ-Wing",
			"Generalized RSTUVWXYZ-Wing"
		};


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="rcc">The RCC used.</param>
		public AlsXzTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Rcc rcc)
			: base(conclusions, views) => Rcc = rcc;


		/// <summary>
		/// Indicates the RCC used.
		/// </summary>
		public Rcc Rcc { get; }

		/// <inheritdoc/>
		public override string Name
		{
			get
			{
				return IsGeneralizedWing(out _, out int? v) switch
				{
					true => InternalNames[v!.Value.CountSet()],
					false => "Almost Locked Sets XZ Rule"
				};
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return IsGeneralizedWing(out _, out int? v) switch
				{
					true => 3.9M + v!.Value.CountSet() * .2M,
					false => 5.5M
				};
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {Rcc} => {elimStr}";
		}

		/// <summary>
		/// Indicates whether this instance is a generalized regular wing.
		/// </summary>
		/// <param name="isAls1Bivalue">
		/// (<see langword="out"/> parameter) Indicates whether the ALS1 is a bivalue cell.
		/// </param>
		/// <param name="value">
		/// (<see langword="out"/> parameter) The candidate mask of another ALS
		/// (non-bivalue cell ALS).
		/// </param>
		private bool IsGeneralizedWing(
			[NotNullWhen(true)] out bool? isAls1Bivalue, [NotNullWhen(true)] out int? value)
		{
			short v1 = Rcc.Als1.RelativePosMask, v2 = Rcc.Als2.RelativePosMask;
			if ((v1 & (v1 - 1)) == 0)
			{
				value = v2;
				isAls1Bivalue = true;
				return true;
			}
			else if ((v2 & (v2 - 1)) == 0)
			{
				value = v1;
				isAls1Bivalue = false;
				return true;
			}
			else
			{
				value = null;
				isAls1Bivalue = null;
				return false;
			}
		}
	}
}
