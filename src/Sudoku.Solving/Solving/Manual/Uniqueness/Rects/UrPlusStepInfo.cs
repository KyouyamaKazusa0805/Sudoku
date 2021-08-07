using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle plus</b> (UR+) or
	/// <b>avoidable rectangle plus</b> (AR+) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TechniqueCode2">The technique code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="ConjugatePairs">All conjugate pairs.</param>
	/// <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
	public record UrPlusStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		Technique TechniqueCode2, int Digit1, int Digit2, int[] Cells, bool IsAvoidable,
		IReadOnlyList<ConjugatePair> ConjugatePairs, int AbsoluteOffset
	) : UrStepInfo(Conclusions, Views, TechniqueCode2, Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
	{
		/// <inheritdoc/>
		public sealed override decimal Difficulty => 4.4M + ConjugatePairs.Count * .2M;

		/// <inheritdoc/>
		public override string? Acronym => IsAvoidable ? "AR (+)" : "UR (+)";

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;

		/// <summary>
		/// Indicates the conjugate pair string.
		/// </summary>
		[FormatItem]
		protected string ConjPairsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				const string separator = ", ";

				var sb = new ValueStringBuilder(stackalloc char[100]);
				sb.AppendRange(ConjugatePairs, separator);

				return sb.ToString();
			}
		}

		[FormatItem]
		private string Prefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugatePairs.Count == 1 ? "a " : string.Empty;
		}

		[FormatItem]
		private string Suffix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugatePairs.Count == 1 ? string.Empty : "s";
		}


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected sealed override string GetAdditional() => $"{Prefix}conjugate pair{Suffix} {ConjPairsStr}";
	}
}
