using System;
using System.Collections.Generic;

namespace Sudoku.Drawing
{
	public sealed partial class View
	{
		private readonly struct Comparer<T> : IEqualityComparer<(Id id, T element)> where T : IEquatable<T>
		{
			public bool Equals((Id id, T element) x, (Id id, T element) y) => GetHashCode(x) == GetHashCode(y);

			public int GetHashCode((Id id, T element) obj) => obj.element.GetHashCode() * 0xC0DE + obj.id.GetHashCode();
		}
	}
}
