using System.Runtime.CompilerServices;
using System.SourceGeneration;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Text.Notation;

namespace Sudoku.Analytics;

/// <summary>
/// Defines a type that can describe a candidate is the correct or wrong digit.
/// </summary>
/// <param name="mask">
/// <inheritdoc cref="IConclusion{TSelf, TMask}.Mask" path="/summary"/>
/// </param>
/// <remarks>
/// <inheritdoc cref="IConclusion{TSelf, TMask}.Mask" path="/remarks"/>
/// </remarks>
[JsonConverter(typeof(Converter))]
[Equals]
[GetHashCode]
[EqualityOperators]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly partial struct Conclusion([DataMember(MemberKinds.Field), HashCodeMember] Mask mask) : IConclusion<Conclusion, Mask>
{
	/// <summary>
	/// Initializes an instance with a conclusion type and a candidate offset.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="candidate">The candidate offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, Candidate candidate) : this((Mask)(((int)type << 10) + candidate))
	{
	}

	/// <summary>
	/// Initializes the <see cref="Conclusion"/> instance via the specified cell, digit and the conclusion type.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, Cell cell, Digit digit) : this((Mask)(((int)type << 10) + cell * 9 + digit))
	{
	}


	/// <inheritdoc/>
	public Cell Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <inheritdoc/>
	public Digit Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <inheritdoc/>
	public Candidate Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask & 1023;
	}

	/// <inheritdoc/>
	public ConclusionType ConclusionType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ConclusionType)(_mask >> 10 & 1);
	}

	/// <inheritdoc/>
	Mask IConclusion<Conclusion, Mask>.Mask => _mask;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ConclusionType conclusionType, out Candidate candidate)
		=> (conclusionType, candidate) = (ConclusionType, Candidate);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ConclusionType conclusionType, out Cell cell, out Digit digit)
		=> ((conclusionType, _), cell, digit) = (this, Cell, Digit);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conclusion other) => _mask == other._mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Conclusion other) => _mask.CompareTo(_mask);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ConclusionNotation.ToString(this);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conclusion operator ~(Conclusion self) => new(self.ConclusionType == Assignment ? Elimination : Assignment, self.Candidate);
}

/// <summary>
/// The file-local type that provides the basic operation for serialization or deserialization for type <see cref="Conclusion"/>.
/// </summary>
file sealed class Converter : JsonConverter<Conclusion>
{
	/// <inheritdoc/>
	public override Conclusion Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetString() switch
		{
			['R' or 'r', var r, 'C' or 'c', var c, .. " = " or " == ", var d] => new(Assignment, (r - '1') * 9 + (c - '1'), d - '1'),
			['R' or 'r', var r, 'C' or 'c', var c, .. " != " or " <> ", var d] => new(Elimination, (r - '1') * 9 + (c - '1'), d - '1'),
			_ => throw new JsonException()
		};

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Conclusion value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}
