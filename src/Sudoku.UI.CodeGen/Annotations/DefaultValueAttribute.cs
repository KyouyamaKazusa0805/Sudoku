namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that marks onto a preference property item to tell the source generator
/// that the attribute will help the source generator to generates the property initialization clause,
/// with the default values.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class DefaultValueAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="DefaultValueAttribute"/> with the specified values.
	/// </summary>
	/// <param name="symbolsAndDefaultValues">The symbols and the default values.</param>
	public DefaultValueAttribute(params string?[] symbolsAndDefaultValues) =>
		SymbolsAndDefaultValues = symbolsAndDefaultValues;


	/// <summary>
	/// Indicates the symbols and the default values corresponding.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The syntax is used a serial of <see cref="string"/>s to combine a compilation symbol
	/// and the default value in this case. For example, we can set the value
	/// <c>"DEBUG", "1.5", "RELEASE", "3"</c> will generate the source code to assign the value
	/// in the parameterless constructor like:
	/// <code><![CDATA[
	/// public Preference()
	/// {
	/// #if DEBUG
	///     Property = 1.5;
	/// #elif RELEASE
	///     Property = 3;
	/// #else
	/// #warning Assigned failed on property 'Property'.
	/// #endif
	/// }
	/// ]]></code>
	/// </para>
	/// <para>
	/// We use the symbol <c><see langword="null"/></c> to tell the source generator
	/// the case is the default case. If you want to assign the default value, just
	/// use <c><see langword="null"/>, <see langword="null"/></c> to assign the property
	/// with the default value <see langword="null"/>:
	/// <code><![CDATA[
	/// public Preference()
	/// {
	/// #if DEBUG
	///     ...
	/// #else
	///     Property = null; // Here.
	/// #endif
	/// }
	/// ]]></code>
	/// </para>
	/// <para>
	/// If you don't want to assign anything at the case, just use the discard symbol <c>"_"</c>.
	/// For example: <c><see langword="null"/>, "_"</c>, will generate:
	/// <code><![CDATA[
	/// public Preference()
	/// {
	/// #if DEBUG
	///     ...
	/// //#else
	/// //    // No assignments here.
	/// #endif
	/// }
	/// ]]></code>
	/// </para>
	/// </remarks>
	public string?[] SymbolsAndDefaultValues { get; }
}
