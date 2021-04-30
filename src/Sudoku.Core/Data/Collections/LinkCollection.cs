using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.CodeGen;
using Sudoku.Data.Extensions;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Provides a collection that contains the chain links.
	/// </summary>
	[DisallowParameterlessConstructor]
	[AutoEquality(nameof(_collection))]
	public readonly ref partial struct LinkCollection
	{
		/// <summary>
		/// The internal collection.
		/// </summary>
		private readonly Span<Link> _collection;


		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public LinkCollection(in Span<Link> collection) : this() => _collection = collection;

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public LinkCollection(IEnumerable<Link> collection) : this() =>
			_collection = collection.ToArray().AsSpan();


		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			switch (_collection.Length)
			{
				case 0:
				{
					return string.Empty;
				}
				case 1:
				{
					return _collection[0].ToString();
				}
				default:
				{
					var links = _collection.ToArray();
					var sb = new ValueStringBuilder(stackalloc char[100]);
					foreach (var (start, _, type) in links)
					{
						sb.Append(new Candidates { start }.ToString());
						sb.Append(type.GetNotation());
					}
					sb.Append(new Candidates { links[^1].EndCandidate }.ToString());

					// Remove redundant digit labels:
					// r1c1(1) == r1c2(1) --> r1c1 == r1c2(1).
					var list = new List<(int Pos, char Value)>();
					for (int i = 0; i < sb.Length; i++)
					{
						if (sb[i] == '(')
						{
							list.Add((i, sb[i + 1]));
							i += 2;
						}
					}

					char digit = list[^1].Value;
					for (int i = list.Count - 1; i >= 1; i--)
					{
						if (list[i - 1].Value == digit)
						{
							sb.Remove(list[i - 1].Pos, 3);
						}

						digit = list[i - 1].Value;
					}

					return sb.ToString();
				}
			}
		}
	}
}
