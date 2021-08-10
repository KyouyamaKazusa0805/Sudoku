using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or <b>avoidable rectangle</b> (AR) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TechniqueCode2">
	/// <para>The technique code.</para>
	/// <para>
	/// Limited by the C# language, here we creates a new property <see cref="TechniqueCode2"/>
	/// to pass the value and assign it to the property <see cref="TechniqueCode"/>. If write code
	/// to place the property <see cref="TechniqueCode"/> into the primary constructor as a parameter,
	/// the default member named <c>TechniqueCode</c> may be duplicate with this parameter's,
	/// which isn't allowed in <see langword="record"/> types in the langugae design.
	/// </para>
	/// </param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
	public abstract record UrStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		Technique TechniqueCode2, int Digit1, int Digit2, int[] Cells, bool IsAvoidable, int AbsoluteOffset
	) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override string? Acronym => IsAvoidable ? "AR" : "UR";

		/// <inheritdoc/>
		public sealed override Technique TechniqueCode => TechniqueCode2;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.Ur;

		/// <summary>
		/// Indicates the digit 1 string.
		/// </summary>
		[FormatItem]
		protected string D1Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit1 + 1).ToString();
		}

		/// <summary>
		/// Indicates the digit 2 string.
		/// </summary>
		[FormatItem]
		protected string D2Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit2 + 1).ToString();
		}

		/// <summary>
		/// Indicates the cells string.
		/// </summary>
		[FormatItem]
		protected string CellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells(Cells).ToString();
		}
	}
}
