namespace Sudoku.Measuring;

/// <summary>
/// Represents an attribute type that describes the current property can be used as a factor.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed partial class FactorPropertyAttribute<TValue> : Attribute
{
	/// <summary>
	/// Indicates the name of the property that will be used in resource dictionary
	/// by using <see cref="ResourceDictionary.Get(string, CultureInfo?, Assembly?)"/>.
	/// </summary>
	/// <seealso cref="ResourceDictionary.Get(string, CultureInfo?, Assembly?)"/>
	public required string? ResourceKey { get; init; }

	/// <summary>
	/// Indicates the minimum value that the property value can be reached.
	/// </summary>
	public TValue? MinValue { get; init; }

	/// <summary>
	/// Indicates the maximum value that the property value can be reached.
	/// </summary>
	public TValue? MaxValue { get; init; }
}
