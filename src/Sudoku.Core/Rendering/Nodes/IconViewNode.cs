using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.SourceGeneration;

namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines an icon view node that applies to a cell, indicating the icon of the cell. The icons can be used on some sudoku variants.
/// </summary>
/// <param name="identifier"><inheritdoc cref="ViewNode(ColorIdentifier)"/></param>
/// <param name="cell">The cell.</param>
[GetHashCode]
[ToString]
public abstract partial class IconViewNode(ColorIdentifier identifier, [Data, HashCodeMember, StringMember] Cell cell) : ViewNode(identifier)
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Cell cell) => cell = Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] ViewNode? other) => other is IconViewNode comparer && Cell == comparer.Cell;
}
