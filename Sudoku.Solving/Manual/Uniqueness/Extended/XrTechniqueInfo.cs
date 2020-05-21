#nullable disable warnings

using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) technique.
	/// </summary>
	public abstract class XrTechniqueInfo : UniquenessTechniqueInfo, IComparable<XrTechniqueInfo>
	{
		/// <summary>
		/// The difficulty extra.
		/// </summary>
		protected static readonly decimal[] DifficultyExtra =
		{
			0, 0, 0, 0, .1M, 0, .2M, 0, .3M, 0, .4M, 0, .5M, 0, .6M
		};

		/// <summary>
		/// The type code.
		/// </summary>
		private readonly int _typeCode;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="typeCode">The type code.</param>
		/// <param name="typeName">The type name.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		public XrTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int typeCode, string typeName, IReadOnlyList<int> cells, IReadOnlyList<int> digits)
			: base(conclusions, views) =>
			(Cells, Digits, _typeCode, TypeName) = (cells, digits, typeCode, typeName);


		/// <summary>
		/// Indicates the cells.
		/// </summary>
		public IReadOnlyList<int> Cells { get; }

		/// <summary>
		/// Indicates all digits.
		/// </summary>
		public IReadOnlyList<int> Digits { get; }

		/// <summary>
		/// Indicates the type name.
		/// </summary>
		public string TypeName { get; }

		/// <summary>
		/// Indicates the size of the instance.
		/// </summary>
		public int Size => Cells.Count >> 1;

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => true;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string? additional = GetAdditional();
			return
				$"{Name}: {digitsStr} in {cellsStr}{(additional is null ? string.Empty : $" with {additional}")} => " +
				$"{elimStr}";
		}

		/// <summary>
		/// Get additional string.
		/// </summary>
		/// <returns>The additional string.</returns>
		protected abstract string? GetAdditional();

		/// <inheritdoc/>
		int IComparable<XrTechniqueInfo>.CompareTo(XrTechniqueInfo other) =>
			_typeCode.CompareTo(other._typeCode);
	}
}
