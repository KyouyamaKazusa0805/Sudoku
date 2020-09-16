using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Sudoku.Constants;
using Sudoku.DocComments;
using Sudoku.Extensions;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Provides a collection with candidates.
	/// </summary>
	public readonly ref struct CandidateCollection
	{
		/// <summary>
		/// The inner map.
		/// </summary>
		private readonly ValueSudokuMap _map;


		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidate">The candidates.</param>
		public CandidateCollection(int candidate)
		{
			_map = new();
			_map.Add(candidate);
		}

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		public CandidateCollection(ReadOnlySpan<int> candidates)
		{
			_map = new();
			_map.AddRange(candidates);
		}

		/// <summary>
		/// Initializes an instance with the specified sudoku map.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		public CandidateCollection(ValueSudokuMap candidates) => _map = candidates;

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		public CandidateCollection(IEnumerable<int> candidates)
		{
			_map = new();
			_map.AddRange(candidates);
		}


		/// <inheritdoc cref="object.Equals(object?)"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override bool Equals(object? obj) => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public bool Equals(CandidateCollection other) => _map == other._map;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override int GetHashCode() => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			int[] candidates = _map.ToArray();
			foreach (var digitGroup in from Candidate in candidates group Candidate by Candidate % 9)
			{
				sb
					.Append(new CellCollection(from Candidate in digitGroup select Candidate / 9).ToString())
					.Append($"({digitGroup.Key + 1}){separator}");
			}

			return sb.Length == 0 ? "{ }" : sb.RemoveFromEnd(separator.Length).ToString();
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(CandidateCollection left, CandidateCollection right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(CandidateCollection left, CandidateCollection right) => !(left == right);
	}
}
