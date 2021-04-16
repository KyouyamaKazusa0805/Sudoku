using System;
using System.Text;
using Sudoku.CodeGen.StructParameterlessConstructor.Annotations;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a link used for drawing.
	/// </summary>
	[DisallowParameterlessConstructor]
	public readonly partial struct Link : IValueEquatable<Link>
	{
		/// <summary>
		/// Initializes an instance with the specified start and end candidate, and a link type.
		/// </summary>
		/// <param name="startCandidate">The start candidate.</param>
		/// <param name="endCandidate">The end candidate.</param>
		/// <param name="linkType">The link type.</param>
		public Link(int startCandidate, int endCandidate, LinkType linkType)
		{
			StartCandidate = startCandidate;
			EndCandidate = endCandidate;
			LinkType = linkType;
		}


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


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="startCandidate">The start candidate.</param>
		/// <param name="endCandidate">The end candidate.</param>
		/// <param name="linkType">The link type.</param>
		public void Deconstruct(out int startCandidate, out int endCandidate, out LinkType linkType)
		{
			startCandidate = StartCandidate;
			endCandidate = EndCandidate;
			linkType = LinkType;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="startCell">The start cell.</param>
		/// <param name="startDigit">The start digit.</param>
		/// <param name="endCell">The end cell.</param>
		/// <param name="endDigit">The end digit.</param>
		/// <param name="linkType">The link type.</param>
		public void Deconstruct(
			out int startCell, out int startDigit, out int endCell, out int endDigit, out LinkType linkType)
		{
			startCell = StartCandidate / 9;
			startDigit = StartCandidate % 9;
			endCell = EndCandidate / 9;
			endDigit = EndCandidate % 9;
			linkType = LinkType;
		}

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			var sb = new ValueStringBuilder(stackalloc char[100]);
			sb.Append(new Candidates { StartCandidate }.ToString());
			sb.Append(LinkType.GetNotation());
			sb.Append(new Candidates { EndCandidate }.ToString());

			return sb.ToString();
		}

		/// <inheritdoc cref="object.Equals(object?)"/>
		public override bool Equals(object? obj) => obj is Link comparer && Equals(comparer);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		public bool Equals(in Link other)
		{
			var (a, b, c) = this;
			var (d, e, f) = other;
			return (int)c << 20 == (int)f << 20 && a << 10 == d << 10 && b == e
				|| (int)c << 20 == (int)f << 20 && b << 10 == e << 10 && a == d;
		}

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => (int)LinkType << 20 | StartCandidate << 10 | EndCandidate;


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in Link left, in Link right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in Link left, in Link right) => !(left == right);
	}
}
