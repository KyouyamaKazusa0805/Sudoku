using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;
using System.Text.Json.Serialization;
using Sudoku.Concepts.Converters;

namespace Sudoku.Concepts;

/// <summary>
/// Defines the data structure that stores a set of cells and a digit, indicating the information
/// about the locked candidate node.
/// </summary>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <remarks>
/// <include file="../../global-doc-comments.xml" path="/g/large-structure"/>
/// </remarks>
[LargeStructure]
[Equals]
[GetHashCode]
[ToString]
[EqualityOperators]
[StructLayout(LayoutKind.Auto)]
[method: JsonConstructor]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly partial struct LockedTarget(
	[Data, HashCodeMember] Digit digit,
	[Data, HashCodeMember, StringMember] CellMap cells
) : IEquatable<LockedTarget>, IEqualityOperators<LockedTarget, LockedTarget, bool>
{
	/// <summary>
	/// Indicates whether the number of cells is 1.
	/// </summary>
	[JsonIgnore]
	public bool IsSole => Cells.Count == 1;

	/// <summary>
	/// The digit string value.
	/// </summary>
	[StringMember(nameof(Digit))]
	private string DigitString => new RxCyConverter().DigitConverter((Mask)(1 << Digit));


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap cells, out Digit digit) => (cells, digit) = (Cells, Digit);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(IEquatable<>))]
	public bool Equals(scoped ref readonly LockedTarget other) => Digit == other.Digit && Cells == other.Cells;
}
