#nullable enable annotations

#pragma warning disable IDE0060
#pragma warning disable CA1034

using System;
using System.Collections.Generic;

namespace Sudoku.DocComments
{
	/// <summary>
	/// Provides with doc comments for exocet eliminations.
	/// </summary>
	public abstract class ExocetElimination
	{
		/// <summary>
		/// Initializes an instance with the specified conclusions.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		protected ExocetElimination(IList<Conclusion> conclusions) => Conclusions = conclusions;


		/// <summary>
		/// Indicates the number of all conclusions.
		/// </summary>
		public abstract int Count { get; }

		/// <summary>
		/// Indicates the conclusions.
		/// </summary>
		public IList<Conclusion>? Conclusions { get; protected set; }


		/// <summary>
		/// Add the conclusion into the collection.
		/// </summary>
		/// <param name="conclusion">The conclusion.</param>
		public void Add(Conclusion conclusion) => throw new NotImplementedException();

		/// <summary>
		/// Add a serial of conclusions into this collection.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		public void AddRange(IEnumerable<Conclusion> conclusions) => throw new NotImplementedException();

		/// <summary>
		/// Merge all eliminations.
		/// </summary>
		/// <param name="eliminations">(<see langword="params"/> parameter) All instances to merge.</param>
		/// <returns>The merged result.</returns>
		public ExocetElimination Merge(params ExocetElimination?[] eliminations) => throw new NotImplementedException();


		/// <summary>
		/// Merge all conclusions.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <returns>The merged result.</returns>
		public static ExocetElimination MergeAll(IEnumerable<ExocetElimination> list) => throw new NotImplementedException();

		/// <summary>
		/// Merge all conclusions.
		/// </summary>
		/// <param name="list">(<see langword="params"/> parameter) The list.</param>
		/// <returns>The merged result.</returns>
		public static ExocetElimination MergeAll(params ExocetElimination[] list) => throw new NotImplementedException();


		/// <summary>
		/// Encapsulates a conclusion same as <c>Sudoku.Data.Conclusion</c>.
		/// </summary>
		public readonly struct Conclusion { }
	}
}
