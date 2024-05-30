namespace Sudoku.Concepts;

/// <summary>
/// Represents extra options that formats a <see cref="CandidateMap"/> instance, or parses into a <see cref="CandidateMap"/> instance.
/// </summary>
/// <seealso cref="CandidateMap"/>
[TypeImpl(
	TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators,
	OtherModifiersOnEquals = "sealed",
	GetHashCodeBehavior = GetHashCodeBehavior.MakeAbstract)]
public abstract partial class CandidateMapFormatInfo :
	IEquatable<CandidateMapFormatInfo>,
	IEqualityOperators<CandidateMapFormatInfo, CandidateMapFormatInfo, bool>,
	IFormatProvider
{
	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public abstract object? GetFormat(Type? formatType);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] CandidateMapFormatInfo? other);

	/// <summary>
	/// Creates a copy of the current instance.
	/// </summary>
	/// <returns>A new instance whose internal values are equal to the current instance.</returns>
	public abstract CandidateMapFormatInfo Clone();

	/// <summary>
	/// Try to format the current map into a valid string result.
	/// </summary>
	/// <param name="map">The map to be formatted.</param>
	/// <returns>The <see cref="string"/> representation of the argument <paramref name="map"/>.</returns>
	protected internal abstract string FormatMap(ref readonly CandidateMap map);

	/// <summary>
	/// Try to parse the specified <see cref="string"/> instance into a valid <see cref="CandidateMap"/>.
	/// </summary>
	/// <param name="str">The string value to be parsed.</param>
	/// <returns>The <see cref="CandidateMap"/> as the result.</returns>
	protected internal abstract CandidateMap ParseMap(string str);
}
