#if false

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Data;

namespace Sudoku.Models
{
	partial record struct PresentationData
	{
		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly partial bool Equals(in PresentationData other) => ToSvgCode() == other.ToSvgCode();

		/// <summary>
		/// Checks whether the collection contains the specified element.
		/// </summary>
		/// <typeparam name="TStruct">The type of the element.</typeparam>
		/// <param name="dataKind">The data kind.</param>
		/// <param name="element">The element to check.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly partial bool Contains<TStruct>(PresentationDataKind dataKind, TStruct element)
			where TStruct : struct => IndexOf(dataKind, element) != -1;

		/// <summary>
		/// Checks whether the collection contains the specified element. If so, return the index of the element.
		/// </summary>
		/// <typeparam name="TStruct">The type of the element.</typeparam>
		/// <param name="dataKind">The data kind.</param>
		/// <param name="element">The element to check.</param>
		/// <returns>An <see cref="int"/> index. If not found, return -1.</returns>
		public readonly partial int IndexOf<TStruct>(PresentationDataKind dataKind, TStruct element)
			where TStruct : struct
		{
			switch (dataKind)
			{
				case PresentationDataKind.Cells when element is int e && Cells is not null:
				{
					int index = -1;
					for (int i = 0, count = Cells.Count; i < count; i++)
					{
						if (Cells[i].Cell == e)
						{
							index = i;
							break;
						}
					}

					return index;
				}
				case PresentationDataKind.Candidates when element is int e && Candidates is not null:
				{
					int index = -1;
					for (int i = 0, count = Candidates.Count; i < count; i++)
					{
						if (Candidates[i].Candidate == e)
						{
							index = i;
							break;
						}
					}

					return index;
				}
				case PresentationDataKind.Regions when element is int e && Regions is not null:
				{
					int index = -1;
					for (int i = 0, count = Regions.Count; i < count; i++)
					{
						if (Regions[i].Region == e)
						{
							index = i;
							break;
						}
					}

					return index;
				}
				case PresentationDataKind.Links when element is Link e && Links is not null:
				{
					int index = -1;
					for (int i = 0, count = Links.Count; i < count; i++)
					{
						if (Links[i].Link == e)
						{
							index = i;
							break;
						}
					}

					return index;
				}
				case PresentationDataKind.DirectLines
				when element is ValueTuple<Cells, Cells> e && DirectLines is not null:
				{
					int index = -1;
					for (int i = 0, count = DirectLines.Count; i < count; i++)
					{
						if (DirectLines[i].DirectLine == e)
						{
							index = i;
							break;
						}
					}

					return index;
				}
				default:
				{
					return -1;
				}
			}
		}

		/// <summary>
		/// Append an element into the collection.
		/// </summary>
		/// <typeparam name="TStruct">The type of the element to add.</typeparam>
		/// <param name="dataKind">The data kind to append.</param>
		/// <param name="element">The element to add.</param>
		/// <param name="color">The color value represented by an <see cref="int"/> value.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws when the argument <paramref name="dataKind"/> is out of range.
		/// </exception>
		public partial void Add<TStruct>(PresentationDataKind dataKind, TStruct element, int color)
			where TStruct : struct
		{
			switch (dataKind)
			{
				case PresentationDataKind.Cells when element is int e:
				{
					var collection = Cells;
					EnsureNotNull(ref collection);
					Cells = collection;
					Cells.Add((e, color));

					break;
				}
				case PresentationDataKind.Candidates when element is int e:
				{
					var collection = Candidates;
					EnsureNotNull(ref collection);
					Candidates = collection;
					Candidates.Add((e, color));

					break;
				}
				case PresentationDataKind.Regions when element is int e:
				{
					var collection = Regions;
					EnsureNotNull(ref collection);
					Regions = collection;
					Regions.Add((e, color));

					break;
				}
				case PresentationDataKind.Links when element is Link e:
				{
					var collection = Links;
					EnsureNotNull(ref collection);
					Links = collection;
					Links.Add((e, color));

					break;
				}
				case PresentationDataKind.DirectLines when element is ValueTuple<Cells, Cells> e:
				{
					var collection = DirectLines;
					EnsureNotNull(ref collection);
					DirectLines = collection;
					DirectLines.Add((e, color));

					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException(
						"<unknown parameter>",
						"The specified argument is invalid."
					);
				}
			}
		}

		/// <summary>
		/// Removes the element out of the collection.
		/// </summary>
		/// <typeparam name="TStruct">The type of the element to remove.</typeparam>
		/// <param name="dataKind">The data kind.</param>
		/// <param name="element">The element.</param>
		public partial void Remove<TStruct>(PresentationDataKind dataKind, TStruct element) where TStruct : struct
		{
			switch (dataKind)
			{
				case PresentationDataKind.Cells when element is int e && Cells is not null:
				{
					int index = -1;
					for (int i = 0, count = Cells.Count; i < count; i++)
					{
						if (Cells[i].Cell == e)
						{
							index = i;
							break;
						}
					}
					if (index != -1)
					{
						Cells.RemoveAt(index);
					}

					break;
				}
				case PresentationDataKind.Candidates when element is int e && Candidates is not null:
				{
					int index = -1;
					for (int i = 0, count = Candidates.Count; i < count; i++)
					{
						if (Candidates[i].Candidate == e)
						{
							index = i;
							break;
						}
					}
					if (index != -1)
					{
						Candidates.RemoveAt(index);
					}

					break;
				}
				case PresentationDataKind.Regions when element is int e && Regions is not null:
				{
					int index = -1;
					for (int i = 0, count = Regions.Count; i < count; i++)
					{
						if (Regions[i].Region == e)
						{
							index = i;
							break;
						}
					}
					if (index != -1)
					{
						Regions.RemoveAt(index);
					}

					break;
				}
				case PresentationDataKind.Links when element is Link e && Links is not null:
				{
					int index = -1;
					for (int i = 0, count = Links.Count; i < count; i++)
					{
						if (Links[i].Link == e)
						{
							index = i;
							break;
						}
					}
					if (index != -1)
					{
						Links.RemoveAt(index);
					}

					break;
				}
				case PresentationDataKind.DirectLines
				when element is ValueTuple<Cells, Cells> e && DirectLines is not null:
				{
					int index = -1;
					for (int i = 0, count = DirectLines.Count; i < count; i++)
					{
						if (DirectLines[i].DirectLine == e)
						{
							index = i;
							break;
						}
					}
					if (index != -1)
					{
						DirectLines.RemoveAt(index);
					}

					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException(
						"<unknown parameter>",
						"The specified argument is invalid."
					);
				}
			}
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly partial int GetHashCode() => ToSvgCode().GetHashCode();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly partial string ToString() => ToSvgCode();

#pragma warning disable IDE0022
		/// <summary>
		/// Fetch the SVG-format <see cref="string"/> instance that can represent this instance.
		/// </summary>
		/// <returns>The SVG-format <see cref="string"/> result.</returns>
		public readonly partial string ToSvgCode()
		{
			throw new NotImplementedException("I'll implement this method later.");
		}
#pragma warning restore IDE0022


#pragma warning disable IDE0022
		/// <summary>
		/// Parse a <see cref="string"/> value which is the SVG-format code
		/// to a <see cref="PresentationData"/> result.
		/// </summary>
		/// <param name="svgCode">The SVG <see cref="string"/> code.</param>
		/// <returns>The result.</returns>
		public static partial PresentationData Parse(string svgCode)
		{
			throw new NotImplementedException("I'll implement this method later.");
		}
#pragma warning restore IDE0022

		/// <summary>
		/// Try to parse a <see cref="string"/> value which is the SVG-format code
		/// to a <see cref="PresentationData"/> result.
		/// </summary>
		/// <param name="svgCode">The SVG <see cref="string"/> code.</param>
		/// <param name="result">
		/// The result. The value keeps <see langword="default"/> value
		/// when the return value is <see langword="false"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> result indicating whether the parse operation is successful.</returns>
		public static partial bool TryParse(string svgCode, out PresentationData result)
		{
			try
			{
				result = Parse(svgCode);
				return true;
			}
			catch (FormatException)
			{
				result = default;
				return false;
			}
		}

		/// <summary>
		/// To ensure the collection isn't <see langword="null"/>. If <see langword="null"/>, the method
		/// will initialize the instance specified as the parameter with the modifier <see langword="ref"/>,
		/// which means the value can be re-assigned in this method.
		/// </summary>
		/// <typeparam name="TStruct">The type of the element.</typeparam>
		/// <param name="collection">The collection to initialize when <see langword="null"/>.</param>
		/// <remarks>
		/// Althogh the parameter type doesn't specify the nullable notation <c>'?'</c>,
		/// but this parameter is marked <see cref="AllowNullAttribute"/>, which means the argument can
		/// also pass a <see langword="null"/>-able type, and changes to a non-<see langword="null"/> value
		/// when the method has been executed wholly.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void EnsureNotNull<TStruct>([AllowNull] ref IList<(TStruct, int)> collection)
			where TStruct : struct => collection ??= new List<(TStruct, int)>();
	}
}

#endif