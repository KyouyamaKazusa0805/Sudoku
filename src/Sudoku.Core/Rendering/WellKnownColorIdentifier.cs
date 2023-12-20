using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using System.Text.Json.Serialization;

namespace Sudoku.Rendering;

/// <summary>
/// Defines a <see cref="ColorIdentifier"/> derived type that uses well-known kinds to distinct with colors.
/// </summary>
/// <param name="kind">The well-known identifier kind to be assigned.</param>
[GetHashCode]
[ToString(ToStringBehavior.RecordLike)]
[method: JsonConstructor]
public sealed partial class WellKnownColorIdentifier([Data, HashCodeMember, StringMember] WellKnownColorIdentifierKind kind) :
	ColorIdentifier
{
	/// <inheritdoc cref="WellKnownColorIdentifierKind.Normal"/>
	public static readonly ColorIdentifier Normal = WellKnownColorIdentifierKind.Normal;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Auxiliary1"/>
	public static readonly ColorIdentifier Auxiliary1 = WellKnownColorIdentifierKind.Auxiliary1;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Auxiliary2"/>
	public static readonly ColorIdentifier Auxiliary2 = WellKnownColorIdentifierKind.Auxiliary2;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Auxiliary3"/>
	public static readonly ColorIdentifier Auxiliary3 = WellKnownColorIdentifierKind.Auxiliary3;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Assignment"/>
	public static readonly ColorIdentifier Assignment = WellKnownColorIdentifierKind.Assignment;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.OverlappedAssignment"/>
	public static readonly ColorIdentifier OverlappedAssignment = WellKnownColorIdentifierKind.OverlappedAssignment;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Elimination"/>
	public static readonly ColorIdentifier Elimination = WellKnownColorIdentifierKind.Elimination;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Cannibalism"/>
	public static readonly ColorIdentifier Cannibalism = WellKnownColorIdentifierKind.Cannibalism;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Exofin"/>
	public static readonly ColorIdentifier Exofin = WellKnownColorIdentifierKind.Exofin;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Endofin"/>
	public static readonly ColorIdentifier Endofin = WellKnownColorIdentifierKind.Endofin;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Link"/>
	public static readonly ColorIdentifier Link = WellKnownColorIdentifierKind.Link;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet1"/>
	public static readonly ColorIdentifier AlmostLockedSet1 = WellKnownColorIdentifierKind.AlmostLockedSet1;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet2"/>
	public static readonly ColorIdentifier AlmostLockedSet2 = WellKnownColorIdentifierKind.AlmostLockedSet2;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet3"/>
	public static readonly ColorIdentifier AlmostLockedSet3 = WellKnownColorIdentifierKind.AlmostLockedSet3;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet4"/>
	public static readonly ColorIdentifier AlmostLockedSet4 = WellKnownColorIdentifierKind.AlmostLockedSet4;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet5"/>
	public static readonly ColorIdentifier AlmostLockedSet5 = WellKnownColorIdentifierKind.AlmostLockedSet5;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out WellKnownColorIdentifierKind kind) => kind = Kind;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ColorIdentifier? other)
		=> other is WellKnownColorIdentifier comparer && Kind == comparer.Kind;
}
