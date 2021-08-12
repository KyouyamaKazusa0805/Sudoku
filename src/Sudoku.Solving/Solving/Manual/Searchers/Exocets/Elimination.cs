using System.Runtime.CompilerServices;
using Sudoku.CodeGenerating;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Resources;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates an instance to represent a series of eliminations in JE or SE.
	/// </summary>
	[AutoDeconstruct(nameof(Eliminations), nameof(Reason))]
	[AutoHashCode(nameof(Eliminations), nameof(Reason))]
	[AutoEquality(nameof(Eliminations), nameof(Reason))]
	[AutoGetEnumerator("@", MemberConversion = "@.AsSpan().*", ReturnType = typeof(ReadOnlySpan<Conclusion>.Enumerator))]
	public readonly partial struct Elimination : IValueEquatable<Elimination>
	{
		/// <summary>
		/// Initializes an instance with the eliminations and the reason.
		/// </summary>
		/// <param name="eliminations">The eliminations.</param>
		/// <param name="reason">The reason why those candidates can be eliminated.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Elimination(in Candidates eliminations, EliminatedReason reason)
		{
			Eliminations = eliminations;
			Reason = reason;
		}


		/// <summary>
		/// Indicates how many eliminations the instance contains.
		/// </summary>
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Eliminations.Count;
		}

		/// <summary>
		/// Indicates the eliminations.
		/// </summary>
		public Candidates Eliminations { get; }

		/// <summary>
		/// Indicates the reason why these candidates can be eliminated.
		/// </summary>
		public EliminatedReason Reason { get; }

		/// <summary>
		/// Indicates the header of the reason.
		/// </summary>
		private string Header => TextResources.Current[$"Exocet{Reason.ToString()}EliminationName"];


		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			string snippet = TextResources.Current.ExocetEliminationSnippet;
			string elim = new ConclusionCollection(AsSpan()).ToString();
			return $"* {Header}{snippet}{elim}";
		}

		/// <summary>
		/// Converts all elements to <see cref="Conclusion"/>.
		/// </summary>
		/// <returns>The <see cref="ReadOnlySpan{T}"/> of type <see cref="Conclusion"/>.</returns>
		/// <seealso cref="ReadOnlySpan{T}"/>
		/// <seealso cref="Conclusion"/>
		public ReadOnlySpan<Conclusion> AsSpan()
		{
			var result = new Conclusion[Eliminations.Count];
			for (int i = 0, count = Eliminations.Count; i < count; i++)
			{
				result[i] = new(ConclusionType.Elimination, Eliminations[i]);
			}

			return result;
		}


		/// <summary>
		/// To merge two different instances, and return the merged result.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>
		/// The merged result. The result will contain all eliminations from two instances, and
		/// the reason should be same.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Throws when two instances contains different eliminated reason.
		/// </exception>
		public static unsafe Elimination operator |(in Elimination left, in Elimination right)
		{
			var (le, lr) = left;
			var (re, rr) = right;

			if (lr != rr)
			{
				throw new ArgumentException("Two arguments should contains same eliminated reason.");
			}

			int totalCount = le.Count + re.Count;
			int* merged = stackalloc int[totalCount];
			for (int i = 0, count = le.Count; i < count; i++)
			{
				merged[i] = left.Eliminations[i];
			}
			for (int i = 0, count = re.Count; i < count; i++)
			{
				merged[i + le.Count] = re[i];
			}

			return new(new(merged, totalCount), lr);
		}
	}
}
