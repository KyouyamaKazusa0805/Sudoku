using System;
using System.Text;
using Sudoku.CodeGenerating;
using Sudoku.Data.Extensions;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a link used for drawing.
	/// </summary>
	[AutoDeconstruct(nameof(StartCandidate), nameof(EndCandidate), nameof(LinkType))]
	[AutoDeconstruct(nameof(StartCell), nameof(StartDigit), nameof(EndCell), nameof(EndDigit), nameof(LinkType))]
	[AutoHashCode(nameof(EigenValue))]
	[AutoEquality(nameof(EigenValue))]
	public readonly partial struct Link : IValueEquatable<Link>, IJsonSerializable<Link, Link.JsonConverter>
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

		/// <summary>
		/// Indicates the start cell.
		/// </summary>
		private int StartCell => StartCandidate / 9;

		/// <summary>
		/// Indicates the start digit.
		/// </summary>
		private int StartDigit => StartCandidate % 9;

		/// <summary>
		/// Indicates the end cell.
		/// </summary>
		private int EndCell => EndCandidate / 9;

		/// <summary>
		/// Indicates the end digit.
		/// </summary>
		private int EndDigit => EndCandidate % 9;

		/// <summary>
		/// Indicates the eigen value.
		/// </summary>
		private int EigenValue => (int)LinkType << 20 | StartCandidate << 10 | EndCandidate;


		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			var sb = new ValueStringBuilder(stackalloc char[100]);
			sb.Append(new Candidates { StartCandidate }.ToString());
			sb.Append(LinkType.GetNotation());
			sb.Append(new Candidates { EndCandidate }.ToString());

			return sb.ToString();
		}
	}
}
