using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	public sealed class Conclusion : ICloneable<Conclusion>, IEnumerable<Candidate>, IEquatable<Conclusion>
	{
		private readonly ISet<Candidate> _candidates = new SortedSet<Candidate>();


		public Conclusion() : this(ConclusionType.Elimination)
		{
		}

		public Conclusion(ConclusionType conclusionType) =>
			ConclusionType = conclusionType;

		public Conclusion(ConclusionType conclusionType, IEnumerable<Candidate> candidates)
			: this(conclusionType) => AddRange(candidates);

		internal Conclusion(ConclusionType conclusionType, ISet<Candidate> candidates)
			: this(conclusionType, (IEnumerable<Candidate>)candidates)
		{
		}


		public int Count => _candidates.Count;

		public ConclusionType ConclusionType { get; }

		public IEnumerable<IGrouping<int, Candidate>> CandidatesGroupByRow =>
			from cand in _candidates group cand by cand.Row;

		public IEnumerable<IGrouping<int, Candidate>> CandidatesGroupByColumn =>
			from cand in _candidates group cand by cand.Column;

		public IEnumerable<IGrouping<int, Candidate>> CandidatesGroupByBlock =>
			from cand in _candidates group cand by cand.Block;


		public void Add(Candidate candidate) => _candidates.Add(candidate);

		public void AddRange(IEnumerable<Candidate> other) =>
			_candidates.UnionWith(other);

		public void Remove(Candidate candidate) => _candidates.Remove(candidate);

		public void IntersectWith(IEnumerable<Candidate> other) =>
			_candidates.IntersectWith(other);

		public void UnionWith(IEnumerable<Candidate> other) =>
			_candidates.UnionWith(other);

		public void ExceptWith(IEnumerable<Candidate> other) =>
			_candidates.ExceptWith(other);

		public void SymmetricExceptWith(IEnumerable<Candidate> other) =>
			_candidates.SymmetricExceptWith(other);

		public void RemoveWhen(Predicate<Candidate> predicate)
		{
			foreach (var candidate in _candidates)
			{
				if (predicate(candidate))
				{
					_candidates.Remove(candidate);
				}
			}
		}

		public void ApplyTo(Grid grid)
		{
			if (ConclusionType == ConclusionType.Assignment)
			{
				foreach (var (cell, digit) in _candidates)
				{
					var cellInfo = grid[cell];
					if (!cellInfo.IsValueCell)
					{
						cellInfo.Value = digit;
						cellInfo.CellType = CellType.Modifiable;
					}
				}
			}
			else // Eliminations.
			{
				foreach (var (cell, digit) in _candidates)
				{
					var cellInfo = grid[cell];
					if (!cellInfo.IsValueCell)
					{
						cellInfo[digit] = false;
					}
				}
			}
		}

		public bool Any() => Count != 0;

		public Conclusion Clone() =>
			new Conclusion(ConclusionType, new SortedSet<Candidate>(_candidates));

		public override bool Equals(object? obj) =>
			obj is Conclusion comparer && Equals(comparer);

		public bool Equals(Conclusion other) => GetHashCode() == other.GetHashCode();

		public override int GetHashCode()
		{
			int result = GetType().GetHashCode();
			foreach (var candidate in _candidates)
			{
				result ^= candidate.GetHashCode();
			}
			return result;
		}

		public override string ToString()
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			foreach (var candidatesGroup in CandidatesGroupByRow)
			{
				foreach (var candidate in candidatesGroup)
				{
					sb.Append($@"r{candidate.Row + 1}c{candidate.Column + 1} {
						(ConclusionType == ConclusionType.Assignment ? "=" : "<>")
					} {candidate.Digit + 1}{separator}");
				}
			}

			return sb.RemoveFromLast(separator.Length).ToString();
		}

		public IEnumerator<Candidate> GetEnumerator() => _candidates.GetEnumerator();

		object ICloneable.Clone() => Clone();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		public static Conclusion operator +(Conclusion candidates, Candidate candidate)
		{
			candidates.Add(candidate);
			return candidates;
		}

		public static Conclusion operator -(Conclusion candidates, Candidate candidate)
		{
			candidates.Remove(candidate);
			return candidates;
		}

		public static Conclusion operator *(
			Conclusion candidates, IEnumerable<Candidate> values)
		{
			candidates.AddRange(values);
			return candidates;
		}

		public static Conclusion operator /(
			Conclusion candidates, Predicate<Candidate> predicate)
		{
			candidates.RemoveWhen(predicate);
			return candidates;
		}

		public static Conclusion operator %(
			Conclusion candidates, Predicate<Candidate> predicate)
		{
			candidates.IntersectWith(candidates.Where(cand => predicate(cand)));
			return candidates;
		}

		public static bool operator ==(Conclusion left, Conclusion right) =>
			left.Equals(right);

		public static bool operator !=(Conclusion left, Conclusion right) =>
			!(left == right);
	}
}
