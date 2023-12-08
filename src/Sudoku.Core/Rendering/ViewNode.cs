using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.SourceGeneration;
using System.Text.Json.Serialization;
using Sudoku.Rendering.Nodes;

namespace Sudoku.Rendering;

/// <summary>
/// Defines a view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="Identifier" path="/summary"/></param>
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType, TypeDiscriminatorPropertyName = "$typeid")]
[JsonDerivedType(typeof(CellViewNode), 0)]
[JsonDerivedType(typeof(CandidateViewNode), 1)]
[JsonDerivedType(typeof(HouseViewNode), 2)]
[JsonDerivedType(typeof(ChuteViewNode), 3)]
[JsonDerivedType(typeof(BabaGroupViewNode), 4)]
[JsonDerivedType(typeof(LinkViewNode), 5)]
[Equals]
[EqualityOperators]
public abstract partial class ViewNode(ColorIdentifier identifier) : IEquatable<ViewNode>, IEqualityOperators<ViewNode, ViewNode, bool>
{
	/// <summary>
	/// Indicates an instance providing with data for describing coloring.
	/// </summary>
	/// <remarks><b>
	/// We cannot change this property into a primary constructor parameter because here attribute <c>[StringMember]</c>
	/// is not supported by derived types, meaning derived types cannot detect this attribute
	/// because it's in primary constructor declaration by a base type.
	/// </b></remarks>
	[StringMember]
	public ColorIdentifier Identifier { get; } = identifier;

	/// <summary>
	/// Indicates the inner identifier to distinct the different types that is derived from <see cref="ViewNode"/>.
	/// </summary>
	/// <seealso cref="ViewNode"/>
	[HashCodeMember]
	[StringMember("EqualityContract")]
	protected string TypeIdentifier => GetType().Name;


	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] ViewNode? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <inheritdoc/>
	public abstract override string ToString();

	/// <summary>
	/// Creates a new <see cref="View"/> instance with same values as the current instance, with independency.
	/// </summary>
	/// <returns>A new <see cref="View"/> instance with same values as the current instance.</returns>
	public abstract ViewNode Clone();
}
