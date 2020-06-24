using Sudoku.Data.Collections;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a link used for drawing.
	/// </summary>
	public readonly struct Link
	{
		/// <summary>
		/// Initializes an instance with the specified start and endcandidate, and a link type.
		/// </summary>
		/// <param name="startCandidate">The start candidate.</param>
		/// <param name="endCandidate">The end candidate.</param>
		/// <param name="linkType">The link type.</param>
		public Link(int startCandidate, int endCandidate, LinkType linkType) =>
			(StartCandidate, EndCandidate, LinkType) = (startCandidate, endCandidate, linkType);


		/// <summary>
		/// The start candidate.
		/// </summary>
		public int StartCandidate { get; }

		/// <summary>
		/// The end candidate.
		/// </summary>
		public int EndCandidate { get; }

		/// <summary>
		/// The link type.
		/// </summary>
		public LinkType LinkType { get; }


		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="startCandidate">(<see langword="out"/> parameter) The start candidate.</param>
		/// <param name="endCandidate">(<see langword="out"/> parameter) The end candidate.</param>
		/// <param name="linkType">(<see langword="out"/> parameter) The link type.</param>
		public void Deconstruct(out int startCandidate, out int endCandidate, out LinkType linkType) =>
			(startCandidate, endCandidate, linkType) = (StartCandidate, EndCandidate, LinkType);

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString()
		{
			string startStr = new CandidateCollection(StartCandidate).ToString();
			string? linkStr = NameAttribute.GetName(LinkType);
			string endStr = new CandidateCollection(EndCandidate).ToString();
			return $"{startStr}{linkStr}{endStr}";
		}
	}
}
