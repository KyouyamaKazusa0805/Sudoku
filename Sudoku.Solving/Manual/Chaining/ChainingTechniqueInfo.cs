using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>chain</b> technique.
	/// </summary>
	public abstract class ChainingTechniqueInfo : TechniqueInfo, IEquatable<ChainingTechniqueInfo?>
	{
		/// <inheritdoc/>
		protected ChainingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views) : base(conclusions, views)
		{
		}


		/// <inheritdoc/>
		public override bool Equals(object? obj) => Equals(obj as ChainingTechniqueInfo);

		/// <inheritdoc/>
		public override bool Equals(TechniqueInfo? other) => Equals(other as ChainingTechniqueInfo);

		/// <inheritdoc/>
		public bool Equals(ChainingTechniqueInfo? other) => InternalEquals(this, other);

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			// Same conclusions hold same hash code.
			var result = new HashCode();
			foreach (var conclusion in Conclusions)
			{
				result.Add(conclusion);
			}

			return result.ToHashCode();
		}


		/// <summary>
		/// Determine whether two <see cref="ChainingTechniqueInfo"/> instances are same.
		/// </summary>
		/// <param name="left">The left one.</param>
		/// <param name="right">The right one.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private static bool InternalEquals(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) =>
			(left is null, right is null) switch
			{
				(true, true) => true,
				(false, false) => left!.GetHashCode() == right!.GetHashCode(),
				_ => false
			};


		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) =>
			InternalEquals(left, right);

		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) => !(left == right);
	}
}
