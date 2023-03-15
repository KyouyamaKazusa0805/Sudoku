namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Defines an attribute that applies to a type derived from <see cref="StepSearcher"/>,
/// to create multiple <see cref="StepSearcher"/>s for the current type, ordering by separated priority value.
/// </summary>
/// <param name="priority">Indicates the priority that will be used for the ordering.</param>
/// <param name="propertyNamesAndValues">The array of property names and their values.</param>
/// <remarks>
/// Please note that this attribute is optional to be marked,
/// meaning it's unnecessary to be used to mark all built-in <see cref="StepSearcher"/>s.
/// </remarks>
/// <seealso cref="StepSearcher"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class SeparatedAttribute(int priority, params object[] propertyNamesAndValues) : StepSearcherMetadataAttribute
{
	/// <summary>
	/// Indicates the priority value that will be used for the ordering.
	/// </summary>
	public int Priority { get; } = priority;

	/// <summary>
	/// Indicates the property names and the values.
	/// </summary>
	public object[] PropertyNamesAndValues { get; } = propertyNamesAndValues;
}
