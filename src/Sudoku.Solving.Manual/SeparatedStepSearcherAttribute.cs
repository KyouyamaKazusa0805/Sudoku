namespace Sudoku.Solving.Manual;

/// <summary>
/// Defines an attribute that applies to an instance having implemented <see cref="IStepSearcher"/>,
/// to create the options that will be used by the module initializer.
/// </summary>
/// <remarks>
/// Please note that the attribute is optional, which means you don't need to use the attribute
/// on all step searchers.
/// </remarks>
/// <seealso cref="IStepSearcher"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class SeparatedStepSearcherAttribute : Attribute, IComparable<SeparatedStepSearcherAttribute>
{
	/// <summary>
	/// Initializes a <see cref="SeparatedStepSearcherAttribute"/> instance via the specified array
	/// of element type <see cref="object"/>, indicating the property names and their values.
	/// </summary>
	/// <param name="priority">Indicates the priority that will be used for the ordering.</param>
	/// <param name="propertyNamesAndValues">The array of property names and their values.</param>
	public SeparatedStepSearcherAttribute(int priority, params object[] propertyNamesAndValues)
	{
		Priority = priority;
		PropertyNamesAndValues = propertyNamesAndValues;
	}


	/// <summary>
	/// Indicates the priority value that will be used for the ordering.
	/// </summary>
	public int Priority { get; }

	/// <summary>
	/// Indicates the property names and the values.
	/// </summary>
	public object[] PropertyNamesAndValues { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable<SeparatedStepSearcherAttribute>.CompareTo(SeparatedStepSearcherAttribute? other) =>
		other is null
			? throw new ArgumentNullException(nameof(other))
			: Priority.CompareTo(other.Priority);
}
