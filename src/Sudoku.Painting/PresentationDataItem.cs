using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Painting
{
	/// <summary>
	/// Indicates a presentation data item.
	/// </summary>
	[Closed]
	public enum PresentationDataItem : byte
	{
		/// <summary>
		/// Indicates the cell list, which corresponds to <see cref="PresentationData.Cells"/>.
		/// </summary>
		/// <seealso cref="PresentationData.Cells"/>
		CellList,

		/// <summary>
		/// Indicates the candidate list, which corresponds to <see cref="PresentationData.Candidates"/>.
		/// </summary>
		/// <seealso cref="PresentationData.Candidates"/>
		CandidateList,

		/// <summary>
		/// Indicates the region list, which corresponds to <see cref="PresentationData.Regions"/>.
		/// </summary>
		/// <seealso cref="PresentationData.Regions"/>
		RegionList,

		/// <summary>
		/// Indicates the link list, which corresponds to <see cref="PresentationData.Links"/>.
		/// </summary>
		/// <seealso cref="PresentationData.Links"/>
		LinkList,

		/// <summary>
		/// Indicates the direct line list, which corresponds to <see cref="PresentationData.DirectLines"/>.
		/// </summary>
		/// <seealso cref="PresentationData.DirectLines"/>
		DirectLineList,

		/// <summary>
		/// Indicates the step sketch list, which corresponds to <see cref="PresentationData.StepSketch"/>.
		/// </summary>
		/// <seealso cref="PresentationData.StepSketch"/>
		StepSketchList
	}
}
