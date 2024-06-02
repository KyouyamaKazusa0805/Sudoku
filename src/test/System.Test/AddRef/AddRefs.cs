
namespace System.Test;

[TestClass]
public class AddRefs
{
	[TestMethod("List_AddRef")]
	public void TestMethod1()
	{
		var list = new List<LargeStructure>();
		list.AddRef(new());

		var newObject = new LargeStructure();
		list.AddRef(in newObject);

		Assert.IsTrue(list.Count == 2);
	}

	[TestMethod("SortedSet_AddRef")]
	public void TestMethod2()
	{
		var set = new SortedSet<LargeStructure>();
		set.AddRef(new());

		var newObject = new LargeStructure();
		set.AddRef(in newObject); // Won't be added because it is equal to 'new()'.

		Assert.IsTrue(set.Count == 1);
	}
}

/// <summary>
/// Represents a large structure.
/// </summary>
[InlineArray(10)]
[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
file struct LargeStructure : IComparable<LargeStructure>, IEquatable<LargeStructure>
{
	/// <summary>
	/// The internal field.
	/// </summary>
	private long _valueField;


	/// <summary>
	/// Initializes a <see cref="LargeStructure"/> instance.
	/// </summary>
	/// <param name="value">The value.</param>
	public LargeStructure(int value) => this[0] = value;


	/// <inheritdoc/>
	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is LargeStructure comparer && Equals(comparer);

	/// <inheritdoc/>
	public readonly bool Equals(LargeStructure other) => this[0] == other[0];

	/// <inheritdoc/>
	public override readonly int GetHashCode() => (int)(this[0] & uint.MaxValue);

	/// <inheritdoc/>
	public readonly int CompareTo(LargeStructure other) => this[0].CompareTo(other[0]);
}
