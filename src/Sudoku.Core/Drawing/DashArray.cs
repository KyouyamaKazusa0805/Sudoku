namespace Sudoku.Drawing;

/// <summary>
/// Defines a dash array of <see cref="double"/> values. The values can be used in UI as dash array of a double collection.
/// </summary>
[JsonConverter(typeof(Converter))]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.EqualityOperators)]
public readonly partial struct DashArray() : IEnumerable<double>, IEquatable<DashArray>, IEqualityOperators<DashArray, DashArray, bool>
{
	/// <summary>
	/// Indicates the invalid value.
	/// </summary>
	public static readonly DashArray InvalidValue = [0];


	/// <summary>
	/// The double values.
	/// </summary>
	private readonly List<double> _doubles = [];


	/// <summary>
	/// Indicates the number of values.
	/// </summary>
	[JsonIgnore]
	public int Count => _doubles.Count;

	[JsonIgnore]
	[StringMember]
	private string ValuesString => $"[{string.Join(", ", _doubles)}]";


	/// <summary>
	/// Adds a new value into the collection.
	/// </summary>
	/// <param name="value">The value.</param>
	public void Add(double value) => _doubles.Add(value);

	/// <inheritdoc/>
	public bool Equals(DashArray other) => _doubles.SequenceEqual(other._doubles);

	/// <summary>
	/// Converts the current collection into an array of <see cref="double"/> values.
	/// </summary>
	/// <returns>An array of <see cref="double"/> values.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double[] ToArray() => [.. _doubles];

	/// <inheritdoc cref="object.GetHashCode"/>
	public override int GetHashCode()
	{
		var result = default(HashCode);
		foreach (var element in _doubles)
		{
			result.Add(element);
		}
		return result.ToHashCode();
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public Enumerator GetEnumerator() => new(_doubles);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _doubles.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<double> IEnumerable<double>.GetEnumerator() => _doubles.AsEnumerable().GetEnumerator();
}
