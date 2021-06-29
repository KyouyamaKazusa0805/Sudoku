using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.CodeGen;
using static System.Numerics.BitOperations;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Indicates a region collection.
	/// </summary>
	[AutoEquality(nameof(_mask))]
	[AutoGetEnumerator(nameof(_mask), MemberConversion = "@.*", ReturnType = typeof(ReadOnlySpan<int>.Enumerator), ExtraNamespaces = new[] { "System.Extensions" })]
	public readonly ref partial struct RegionCollection
	{
		/// <summary>
		/// The inner mask.
		/// </summary>
		private readonly int _mask;


		/// <summary>
		/// Initializes an empty collection and add one region into the list.
		/// </summary>
		/// <param name="region">The region.</param>
		public RegionCollection(int region) : this() => _mask |= 1 << region;

		/// <summary>
		/// Initializes an instance with the specified regions.
		/// </summary>
		/// <param name="regions">The regions.</param>
		public RegionCollection(in ReadOnlySpan<int> regions) : this()
		{
			foreach (int region in regions)
			{
				_mask |= 1 << region;
			}
		}

		/// <summary>
		/// Initializes an instance with the specified regions.
		/// </summary>
		/// <param name="regions">The regions.</param>
		public RegionCollection(IEnumerable<int> regions) : this()
		{
			foreach (int region in regions)
			{
				_mask |= 1 << region;
			}
		}


		/// <summary>
		/// Indicates the number of regions that contain in this collection.
		/// </summary>
		public int Count => PopCount((uint)_mask);


		/// <summary>
		/// Gets a <see cref="bool"/> value indicating whether the bit of the corresponding specified region
		/// is set <see langword="true"/>.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool this[int region] => (_mask >> region & 1) != 0;


		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => _mask;

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			return Count switch
			{
				0 => string.Empty,
				1 when TrailingZeroCount(_mask) is var r => $"{GetLabel(r / 9)}{(r % 9 + 1).ToString()}",
				_ => f(this)
			};

			static string f(in RegionCollection @this)
			{
				var dic = new Dictionary<int, ICollection<int>>();
				foreach (int region in @this)
				{
					if (!dic.ContainsKey(region / 9))
					{
						dic.Add(region / 9, new List<int>());
					}

					dic[region / 9].Add(region % 9);
				}

				var sb = new ValueStringBuilder(stackalloc char[30]);
				for (int i = 1, j = 0; j < 3; i = (i + 1) % 3, j++)
				{
					if (!dic.ContainsKey(i))
					{
						continue;
					}

					sb.Append(@this.GetLabel(i));
					foreach (int z in dic[i])
					{
						sb.Append(z + 1);
					}
				}

				return sb.ToString();
			}
		}

		/// <summary>
		/// To string but only output the labels ('r', 'c' or 'b').
		/// </summary>
		/// <returns>The labels.</returns>
		public string ToSimpleString()
		{
			var sb = new ValueStringBuilder(stackalloc char[27]);
			for (int region = 9, i = 0; i < 27; i++, region = (region + 1) % 27)
			{
				if (this[region])
				{
					sb.Append(GetLabel(region / 9));
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Get the label of each region.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The label.</returns>
		private char GetLabel(int index) => index switch { 0 => 'b', 1 => 'r', 2 => 'c' };
	}
}
