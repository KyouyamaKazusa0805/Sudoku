namespace Sudoku.Analytics;

/// <summary>
/// To mark onto a step searcher, to tell the runtime and the compiler that the type is a step searcher.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class StepSearcherAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="StepSearcherAttribute"/> instance.
	/// </summary>
	public StepSearcherAttribute()
	{
	}

	/// <summary>
	/// Initializes a <see cref="StepSearcherAttribute"/> instance via the resource entry.
	/// </summary>
	/// <param name="nameResourceEntry">The resource entry.</param>
	public StepSearcherAttribute(string? nameResourceEntry) => NameResourceEntry = nameResourceEntry;


	/// <summary>
	/// Indicates the name stored in resource dictionary, specified as its entry (i.e. resource key).
	/// </summary>
	public string? NameResourceEntry { get; }
}
