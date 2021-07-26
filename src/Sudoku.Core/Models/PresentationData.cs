#if false

#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Data;

namespace Sudoku.Models
{
	/// <summary>
	/// Indicates the presentation data that is used for present the information.
	/// </summary>
	/// <param name="Cells">The cells.</param>
	/// <param name="Candidates">The candidates.</param>
	/// <param name="Regions">The regions.</param>
	/// <param name="Links">The links.</param>
	/// <param name="DirectLines">The direct lines.</param>
	public partial record struct PresentationData(
		[DisallowNull] IList<(int Cell, int Color)>? Cells,
		[DisallowNull] IList<(int Candidate, int Color)>? Candidates,
		[DisallowNull] IList<(int Region, int Color)>? Regions,
		[DisallowNull] IList<(Link Link, int Color)>? Links,
		[DisallowNull] IList<((Cells Start, Cells End) DirectLine, int Color)>? DirectLines
	) : IValueEquatable<PresentationData>, IParsable<PresentationData>
	{
		public readonly partial bool Equals(in PresentationData other);
		public readonly partial bool Contains<TStruct>(PresentationDataKind dataKind, TStruct element) where TStruct : struct;
		public readonly partial int IndexOf<TStruct>(PresentationDataKind dataKind, TStruct element) where TStruct : struct;
		public override readonly partial int GetHashCode();
		public override readonly partial string ToString();
		public readonly partial string ToSvgCode();
		public partial void Add<TStruct>(PresentationDataKind dataKind, TStruct element, int color) where TStruct : struct;
		public partial void Remove<TStruct>(PresentationDataKind dataKind, TStruct element) where TStruct : struct;

		public static partial bool TryParse(string svgCode, out PresentationData result);
		public static partial PresentationData Parse(string svgCode);

		public static bool operator ==(in PresentationData left, in PresentationData right) => left.Equals(in right);
		public static bool operator !=(in PresentationData left, in PresentationData right) => !(left == right);
	}
}

#endif