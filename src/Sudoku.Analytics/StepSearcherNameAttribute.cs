namespace Sudoku.Analytics;

/// <summary>
/// Represents an attribute type that is applied to a <see cref="StepSearcher"/>,
/// indicating the name of the step searcher. This attribute will take effects on <see cref="StepSearcher.ToString"/>.
/// </summary>
/// <seealso cref="StepSearcher.ToString"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StepSearcherNameAttribute : StepSearcherMetadataItemAttribute
{
	/// <summary>
	/// Initiaizes a <see cref="StepSearcherNameAttribute"/> instance.
	/// </summary>
	public StepSearcherNameAttribute()
	{
	}

	/// <summary>
	/// Initializes a <see cref="StepSearcherNameAttribute"/> instance via the specified name.
	/// </summary>
	/// <param name="name">The name.</param>
	public StepSearcherNameAttribute(string name) => Name = name;


	/// <inheritdoc/>
	public override bool IsRequired => false;

	/// <summary>
	/// Indicates whether user specifies for the name of the step searcher.
	/// </summary>
	[MemberNotNullWhen(true, nameof(Name))]
	public bool IsNamedValue => Name is not null;

	/// <summary>
	/// Indicates whether user specifies for the resource entry of the step searcher's name.
	/// </summary>
	[MemberNotNullWhen(true, nameof(ResourceEntry))]
	public bool IsResourceValue => ResourceEntry is not null;

	/// <summary>
	/// Indicates the reflected name of the step searcher's name.
	/// </summary>
	public string? Name { get; init; }

	/// <summary>
	/// Indicates the resource entry where the current step searcher's name is stored.
	/// </summary>
	public string? ResourceEntry { get; init; }
}
