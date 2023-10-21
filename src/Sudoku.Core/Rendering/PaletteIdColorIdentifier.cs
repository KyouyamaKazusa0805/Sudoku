using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using System.Text.Json.Serialization;

namespace Sudoku.Rendering;

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses palette ID value to distinct with colors.
/// </summary>
/// <param name="value">The palette color ID value to be assigned. The color palette requires implementation of target projects.</param>
[GetHashCode]
[ToString(ToStringBehavior.RecordLike)]
[method: JsonConstructor]
public sealed partial class PaletteIdColorIdentifier([DataMember, HashCodeMember, StringMember] Offset value) : ColorIdentifier
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ColorIdentifier? other)
		=> other is PaletteIdColorIdentifier comparer && Value == comparer.Value;
}
