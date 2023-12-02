using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using System.Text.Json.Serialization;
using Sudoku.Text.Converters;

namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a Baba group.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell">Indicates the cell used.</param>
/// <param name="digitsMask">Indicates a mask that hold digits used.</param>
/// <param name="unknownValueChar">Indicates the character that represents the baba group name.</param>
[GetHashCode]
[ToString]
[method: JsonConstructor]
public sealed partial class BabaGroupViewNode(
	ColorIdentifier identifier,
	[Data, HashCodeMember] Cell cell,
	[Data, StringMember] Utf8Char unknownValueChar,
	[Data] Mask digitsMask
) : BasicViewNode(identifier)
{
	/// <summary>
	/// Initializes a <see cref="BabaGroupViewNode"/> instance via the specified values.
	/// </summary>
	/// <inheritdoc cref="BabaGroupViewNode(ColorIdentifier, Cell, Utf8Char, Mask)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BabaGroupViewNode(Cell cell, Utf8Char unknownValueChar, Mask digitsMask) :
		this(WellKnownColorIdentifier.Normal, cell, unknownValueChar, digitsMask)
	{
	}


	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[StringMember(nameof(Cell))]
	private string CellString => new RxCyConverter().CellConverter([Cell]);

	/// <summary>
	/// Indicates the digits mask string.
	/// </summary>
	[StringMember(nameof(DigitsMask))]
	private string DigitsMaskString => Convert.ToString(DigitsMask, 2).ToString();


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out Cell cell, out Utf8Char unknownValueChar)
		=> (identifier, cell, unknownValueChar) = (Identifier, Cell, UnknownValueChar);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out Cell cell, out Mask digitsMask, out Utf8Char unknownValueChar)
		=> ((identifier, cell, unknownValueChar), digitsMask) = (this, DigitsMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is BabaGroupViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BabaGroupViewNode Clone() => new(Identifier, Cell, UnknownValueChar, DigitsMask);
}
