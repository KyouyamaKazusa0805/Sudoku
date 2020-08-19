using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Sudoku.Linq
{
	public static class Queryable
	{
		[return: MaybeNull]
		public static TElement Min<TElement, TComparable>(
			this IQueryable<TElement> elements, Func<TElement, IComparable<TComparable>> selector)
		{
			Contract.Assume(!(elements is null));

			return (
				from element in elements
				orderby selector(element) ascending
				select element
			).FirstOrDefault();
		}

		public static int Count<TElement>(
			this IQueryable<TElement> elements, Func<TElement, int> countingFormula)
		{
			Contract.Assume(!(elements is null));

			int count = 0;
			foreach (var element in elements)
			{
				count += countingFormula(element);
			}

			return count;
		}

		public static int Count<TElement>(
			this IQueryable<TElement> elements,
			Predicate<TElement> selector, Func<TElement, int> countingFormula)
		{
			Contract.Assume(!(elements is null));

			int count = 0;
			foreach (var element in elements)
			{
				if (selector(element))
				{
					count += countingFormula(element);
				}
			}

			return count;
		}
	}
}
