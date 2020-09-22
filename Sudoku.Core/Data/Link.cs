using System;
using Sudoku.DocComments;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a link used for drawing.
	/// </summary>
	public /*readonly*/ struct Link : IEquatable<Link>
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
		public int StartCandidate { get; init; }

		/// <summary>
		/// The end candidate.
		/// </summary>
		public int EndCandidate { get; init; }

		/// <summary>
		/// The link type.
		/// </summary>
		public LinkType LinkType { get; init; }


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="startCandidate">(<see langword="out"/> parameter) The start candidate.</param>
		/// <param name="endCandidate">(<see langword="out"/> parameter) The end candidate.</param>
		public void Deconstruct(out int startCandidate, out int endCandidate) =>
			(startCandidate, endCandidate) = (StartCandidate, EndCandidate);

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="startCandidate">(<see langword="out"/> parameter) The start candidate.</param>
		/// <param name="endCandidate">(<see langword="out"/> parameter) The end candidate.</param>
		/// <param name="linkType">(<see langword="out"/> parameter) The link type.</param>
		public void Deconstruct(out int startCandidate, out int endCandidate, out LinkType linkType) =>
			(startCandidate, endCandidate, linkType) = (StartCandidate, EndCandidate, LinkType);

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="startCell">(<see langword="out"/> parameter) The start cell.</param>
		/// <param name="startDigit">(<see langword="out"/> parameter) The start digit.</param>
		/// <param name="endCell">(<see langword="out"/> parameter) The end cell.</param>
		/// <param name="endDigit">(<see langword="out"/> parameter) The end digit.</param>
		/// <param name="linkType">(<see langword="out"/> parameter) The link type.</param>
		public void Deconstruct(
			out int startCell, out int startDigit, out int endCell, out int endDigit, out LinkType linkType) =>
			(startCell, startDigit, endCell, endDigit, linkType) = (
				StartCandidate / 9, StartCandidate % 9, EndCandidate / 9, EndCandidate % 9, LinkType);

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			string startStr = new SudokuMap { StartCandidate }.ToString();
			string? linkStr = NameAttribute.GetName(LinkType);
			string endStr = new SudokuMap { EndCandidate }.ToString();
			return $"{startStr}{linkStr}{endStr}";
		}

		/// <inheritdoc cref="object.Equals(object?)"/>
		public override bool Equals(object? obj) => obj is Link comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Link other)
		{
			var (a, b, c) = this;
			var (d, e, f) = other;
			return ((int)c << 20 | a << 10 | b) == ((int)f << 20 | d << 10 | e)
				|| ((int)c << 20 | b << 10 | a) == ((int)f << 20 | e << 10 | d);
		}

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => (int)LinkType << 20 | StartCandidate << 10 | EndCandidate;


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(Link left, Link right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(Link left, Link right) => !(left == right);
	}
}
