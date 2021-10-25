extern alias extended_system;

namespace Sudoku.Models;

/// <summary>
/// Indicates the presentation data that is used for present the information.
/// </summary>
/// <param name="Cells">The cells.</param>
/// <param name="Candidates">The candidates.</param>
/// <param name="Regions">The regions.</param>
/// <param name="Links">The links.</param>
/// <param name="DirectLines">The direct lines.</param>
/// <param name="UnknownValues">The unknown values.</param>
public partial record struct PresentationData(
	IList<(int Cell, ColorIdentifier Color)>? Cells,
	IList<(int Candidate, ColorIdentifier Color)>? Candidates,
	IList<(int Region, ColorIdentifier Color)>? Regions,
	IList<(ChainLink Link, ColorIdentifier Color)>? Links,
	IList<(Crosshatch DirectLine, ColorIdentifier Color)>? DirectLines,
	IList<(UnknownValue UnknownValue, ColorIdentifier Color)>? UnknownValues
) : IValueEquatable<PresentationData>, extended_system::System.IParseable<PresentationData>
{
	/// <summary>
	/// Indicates the default instance of this type.
	/// </summary>
	public static readonly PresentationData Undefined;


	/// <summary>
	/// Indicates whether the current instance is undefined.
	/// </summary>
	public bool IsUndefiened => Equals(in Undefined);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in PresentationData other) => ToSvgCode() == other.ToSvgCode();

	/// <summary>
	/// Checks whether the collection contains the specified element.
	/// </summary>
	/// <typeparam name="TStruct">The type of the element.</typeparam>
	/// <param name="dataKind">The data kind.</param>
	/// <param name="element">The element to check.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains<TStruct>(PresentationDataKind dataKind, TStruct element)
	where TStruct : struct =>
		IndexOf(dataKind, element) != -1;

	/// <summary>
	/// Checks whether the collection contains the specified element. If so, return the index of the element.
	/// </summary>
	/// <typeparam name="TStruct">The type of the element.</typeparam>
	/// <param name="dataKind">The data kind.</param>
	/// <param name="element">The element to check.</param>
	/// <returns>An <see cref="int"/> index. If not found, return -1.</returns>
	public readonly int IndexOf<TStruct>(PresentationDataKind dataKind, TStruct element) where TStruct : struct
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
			case PresentationDataKind.Links when element is ChainLink e && Links is not null:
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
			case PresentationDataKind.DirectLines when element is Crosshatch e && DirectLines is not null:
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
			case PresentationDataKind.UnknownValue when element is UnknownValue e && UnknownValues is not null:
			{
				int index = -1;
				for (int i = 0, count = UnknownValues.Count; i < count; i++)
				{
					if (UnknownValues[i].UnknownValue == e)
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
	/// Determines whether the unknown identifiers of the current instance
	/// overlaps the specified cell. Of course the property <see cref="UnknownValues"/>
	/// shouldn't be <see langword="null"/>.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <seealso cref="UnknownValues"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool UnknownIdentifierOverlapsWithCell(int cell) =>
		UnknownValues?.Any(u => u.UnknownValue.Cell == cell) ?? false;

	/// <summary>
	/// Append an element into the collection.
	/// </summary>
	/// <typeparam name="TStruct">The type of the element to add.</typeparam>
	/// <param name="dataKind">The data kind to append.</param>
	/// <param name="element">The element to add.</param>
	/// <param name="color">The color identifier.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="dataKind"/> is out of range.
	/// </exception>
	public void Add<TStruct>(PresentationDataKind dataKind, TStruct element, ColorIdentifier color)
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
			case PresentationDataKind.Links when element is ChainLink e:
			{
				var collection = Links;
				EnsureNotNull(ref collection);
				Links = collection;
				Links.Add((e, color));

				break;
			}
			case PresentationDataKind.DirectLines when element is Crosshatch e:
			{
				var collection = DirectLines;
				EnsureNotNull(ref collection);
				DirectLines = collection;
				DirectLines.Add((e, color));

				break;
			}
			case PresentationDataKind.UnknownValue when element is UnknownValue e:
			{
				var collection = UnknownValues;
				EnsureNotNull(ref collection);
				UnknownValues = collection;
				UnknownValues.Add((e, color));

				break;
			}
			default:
			{
				throw new ArgumentOutOfRangeException("<unknown parameter>", "The specified argument is invalid.");
			}
		}
	}

	/// <summary>
	/// Removes the element out of the collection.
	/// </summary>
	/// <typeparam name="TStruct">The type of the element to remove.</typeparam>
	/// <param name="dataKind">The data kind.</param>
	/// <param name="element">The element.</param>
	public void Remove<TStruct>(PresentationDataKind dataKind, TStruct element) where TStruct : struct
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
			case PresentationDataKind.Links when element is ChainLink e && Links is not null:
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
			case PresentationDataKind.DirectLines when element is Crosshatch e && DirectLines is not null:
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
			case PresentationDataKind.UnknownValue when element is UnknownValue e && UnknownValues is not null:
			{
				int index = -1;
				for (int i = 0, count = UnknownValues.Count; i < count; i++)
				{
					if (UnknownValues[i].UnknownValue == e)
					{
						index = i;
						break;
					}
				}
				if (index != -1)
				{
					UnknownValues.RemoveAt(index);
				}

				break;
			}
			default:
			{
				throw new ArgumentOutOfRangeException("<unknown parameter>", "The specified argument is invalid.");
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => ToSvgCode().GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => ToSvgCode();

#pragma warning disable IDE0022
	/// <summary>
	/// Fetch the SVG-format <see cref="string"/> instance that can represent this instance.
	/// </summary>
	/// <returns>The SVG-format <see cref="string"/> result.</returns>
	public readonly string ToSvgCode()
	{
		throw new NotImplementedException("I'll implement this method later.");
	}
#pragma warning restore IDE0022


#pragma warning disable IDE0022
	/// <inheritdoc/>
	public static PresentationData Parse(string? str)
	{
		throw new NotImplementedException("I'll implement this method later.");
	}
#pragma warning restore IDE0022

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? str, [DiscardWhen(false)] out PresentationData result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (Exception ex) when (ex is ArgumentNullException or FormatException)
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
	/// The argument can be passed a <see langword="null"/>-able type,
	/// and changes to a non-<see langword="null"/> value when the method has been executed wholly.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void EnsureNotNull<TStruct>([AllowNull] ref IList<(TStruct, ColorIdentifier)> collection)
	where TStruct : struct =>
		collection ??= new List<(TStruct, ColorIdentifier)>();

	/// <inheritdoc/>
	public static bool operator ==(in PresentationData left, in PresentationData right) => left.Equals(in right);

	/// <inheritdoc/>
	public static bool operator !=(in PresentationData left, in PresentationData right) => !left.Equals(in right);
}
