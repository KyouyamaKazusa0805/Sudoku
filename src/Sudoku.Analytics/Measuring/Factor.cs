namespace Sudoku.Measuring;

/// <summary>
/// Represents a factor that describes a rule for calculating difficulty rating for a step in one factor.
/// </summary>
public abstract class Factor : ICultureFormattable
{
	/// <summary>
	/// Indicates the scale value to be set. The value will be used by scaling formula. By default, the value will be 0.1.
	/// </summary>
	public decimal Scale { get; protected internal set; } = .1M;

	/// <summary>
	/// Indicates the name of the factor that can be used by telling with multple <see cref="Factor"/>
	/// instances with different types.
	/// </summary>
	public virtual string DistinctKey => GetType().Name;

	/// <summary>
	/// Provides with a formula that calculates for the result, unscaled.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Please note that the return type of this property is <see cref="Expression{TDelegate}"/>,
	/// meaning it is built via LINQ querying mechanism called
	/// "<see href="https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/expression-trees/">Expression Trees</see>",
	/// allowing building C# expressions dynamically.
	/// </para>
	/// <para>
	/// The rule for constructing the property value is simple: just use lambda expressions. By creating a lambda expression,
	/// you can assign the property with the target value, with default scale value 1. For example:
	/// <code><![CDATA[
	/// public override Expression<Func<decimal>> Formula => () => A002024(SubsetSize);
	/// ]]></code>
	/// </para>
	/// </remarks>
	/// <seealso cref="Expression{TDelegate}"/>
	public abstract Expression<Func<decimal>> Formula { get; }


	/// <summary>
	/// Try to fetch the name stored in resource dictionary.
	/// </summary>
	/// <param name="culture">The culture.</param>
	/// <returns>The name of the factor.</returns>
	public virtual string GetName(CultureInfo? culture = null) => ResourceDictionary.Get($"Factor_{DistinctKey}", culture);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString() => ToString(null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CultureInfo? culture = null) => $"{GetName(culture)}: {Formula}";
}
