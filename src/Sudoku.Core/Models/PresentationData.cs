#pragma warning disable CS1591

namespace Sudoku.Models;

/// <summary>
/// Indicates the presentation data that is used for present the information.
/// </summary>
/// <param name="Cells">The cells.</param>
/// <param name="Candidates">The candidates.</param>
/// <param name="Regions">The regions.</param>
/// <param name="Links">The links.</param>
/// <param name="DirectLines">The direct lines.</param>
public partial record struct PresentationData(
	[DisallowNull][property: DisallowNull] IList<(int Cell, ColorIdentifier Color)>? Cells,
	[DisallowNull][property: DisallowNull] IList<(int Candidate, ColorIdentifier Color)>? Candidates,
	[DisallowNull][property: DisallowNull] IList<(int Region, ColorIdentifier Color)>? Regions,
	[DisallowNull][property: DisallowNull] IList<(Link Link, ColorIdentifier Color)>? Links,
	[DisallowNull][property: DisallowNull] IList<(Crosshatch DirectLine, ColorIdentifier Color)>? DirectLines
) : IValueEquatable<PresentationData>/*, IParsable<PresentationData>*/
{
	public readonly partial bool Equals(in PresentationData other);
	public readonly partial bool Contains<TStruct>(PresentationDataKind dataKind, TStruct element) where TStruct : struct;
	public readonly partial int IndexOf<TStruct>(PresentationDataKind dataKind, TStruct element) where TStruct : struct;
	public override readonly partial int GetHashCode();
	public override readonly partial string ToString();
	public readonly partial string ToSvgCode();
	public partial void Add<TStruct>(PresentationDataKind dataKind, TStruct element, ColorIdentifier color) where TStruct : struct;
	public partial void Remove<TStruct>(PresentationDataKind dataKind, TStruct element) where TStruct : struct;

	public static partial bool TryParse(string svgCode, out PresentationData result);
	public static partial PresentationData Parse(string svgCode);

	public static bool operator ==(in PresentationData left, in PresentationData right) => left.Equals(in right);
	public static bool operator !=(in PresentationData left, in PresentationData right) => !(left == right);
}
