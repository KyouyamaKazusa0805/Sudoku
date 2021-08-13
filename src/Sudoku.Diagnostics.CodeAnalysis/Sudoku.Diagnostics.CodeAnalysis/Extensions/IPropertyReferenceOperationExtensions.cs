namespace Microsoft.CodeAnalysis.Operations;

/// <summary>
/// Provides extension methods on <see cref="IPropertyReferenceOperation"/>.
/// </summary>
/// <seealso cref="IPropertyReferenceOperation"/>
public static class IPropertyReferenceOperationExtensions
{
	/// <summary>
	/// Checks whether the current instance has the same reference with the specified one.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <param name="other">Another instance.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool SameReferenceWith(
		this IPropertyReferenceOperation @this, IPropertyReferenceOperation other)
	{
		IPropertySymbol property1 = @this.Property, property2 = other.Property;
		return property1.ToDisplayString() == property2.ToDisplayString();
	}
}
