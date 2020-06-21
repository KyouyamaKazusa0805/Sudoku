using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Sudoku.Constants;
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
		private readonly SudokuMap _map;


		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidate">The candidates.</param>
		public CandidateCollection(int candidate)
		{
			_map = SudokuMap.Empty.Clone();
			_map.Add(candidate);
		}

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		public CandidateCollection(ReadOnlySpan<int> candidates)
		{
			_map = SudokuMap.Empty.Clone();
			_map.AddRange(candidates);
		}

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		public CandidateCollection(IEnumerable<int> candidates)
		{
			_map = SudokuMap.Empty.Clone();
			_map.AddRange(candidates);
		}


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override bool Equals(object? obj) => throw Throwings.RefStructNotSupported;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		public bool Equals(CandidateCollection other) => _map == other._map;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override int GetHashCode() => throw Throwings.RefStructNotSupported;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString()
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			int[] candidates = _map.ToArray();
			foreach (var digitGroup in from candidate in candidates group candidate by candidate % 9)
			{
				sb
					.Append(new CellCollection(from candidate in digitGroup select candidate / 9).ToString())
					.Append($"({digitGroup.Key + 1}){separator}");
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(CandidateCollection left, CandidateCollection right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(CandidateCollection left, CandidateCollection right) => !(left == right);
	}
}
