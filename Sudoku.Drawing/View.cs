using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Models;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// </summary>
	public sealed class View : IEquatable<View>
	{
		/// <summary>
		/// Indicates all cells used.
		/// </summary>
		public IReadOnlyList<DrawingInfo>? Cells { get; init; }

		/// <summary>
		/// Indicates all candidates used.
		/// </summary>
		public IReadOnlyList<DrawingInfo>? Candidates { get; init; }

		/// <summary>
		/// Indicates all regions used.
		/// </summary>
		public IReadOnlyList<DrawingInfo>? Regions { get; init; }

		/// <summary>
		/// Indicates all links used.
		/// </summary>
		public IReadOnlyList<Link>? Links { get; init; }

		/// <summary>
		/// Indicates all direct lines.
		/// </summary>
		public IReadOnlyList<(Cells Start, Cells End)>? DirectLines { get; init; }


		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is View comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(View? other)
		{
			if (other is null
				|| Cells is null ^ other.Cells is null
				|| Candidates is null ^ other.Candidates is null
				|| Regions is null ^ other.Regions is null
				|| Links is null ^ other.Links is null
				|| DirectLines is null ^ other.DirectLines is null)
			{
				return false;
			}

#nullable disable warnings
			return (Cells is null || !Cells.Any(c => !other.Cells.Contains(c)))
				&& (Candidates is null || !Candidates.Any(c => !other.Candidates.Contains(c)))
				&& (Regions is null || !Regions.Any(c => !other.Regions.Contains(c)))
				&& (Links is null || !Links.Any(l => !other.Links.Contains(l)))
				&& (DirectLines is null || !DirectLines.Any(d => !other.DirectLines.Contains(d)));
#nullable restore warnings
			// You can also implement this method in this way.
			//return GetHashCode() == other.GetHashCode();
		}

		/// <inheritdoc/>
		public override int GetHashCode() => ToJson().GetHashCode();

		/// <summary>
		/// Converts the current instance to a JSON string.
		/// </summary>
		/// <param name="options">The option instance.</param>
		/// <returns>The JSON result string.</returns>
		public string ToJson(JsonSerializerOptions? options = null) => JsonSerializer.Serialize(this, options);


#nullable disable warnings
		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(View? left, View? right) =>
			(left, right) switch
			{
				(null, null) => true,
				(not null, not null) => left.Equals(right),
				_ => false
			};
#nullable restore warnings

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(View? left, View? right) => !(left == right);
	}
}
