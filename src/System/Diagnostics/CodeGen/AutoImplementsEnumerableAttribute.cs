namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on automatically implementing
/// <see cref="IEnumerable{T}"/>.
/// </summary>
/// <seealso cref="IEnumerable{T}"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class AutoImplementsEnumerableAttribute :
	Attribute,
	ITypeBinder,
	IInterfaceImplementingCaseController,
	ISingleMemberBinder,
	IPatternProvider
{
	/// <summary>
	/// Initializes an <see cref="AutoImplementsEnumerableAttribute"/> instance via the specified element type
	/// and the member name that is the base instance to get its enumerator as the type's enumerator instance.
	/// </summary>
	/// <param name="elementType">The element type.</param>
	/// <param name="memberName">The member name.</param>
	public AutoImplementsEnumerableAttribute(Type elementType, string? memberName = null)
		=> (Type, MemberName) = (elementType, memberName);


	/// <inheritdoc/>
	public bool UseExplicitImplementation { get; init; } = false;

	/// <summary>
	/// <para>
	/// Indicates the conversion expression. The default value is <c>"*"</c>. The conversion expression
	/// uses the C# basic expression, symbol <c>!</c>, <c>@</c> and <c>*</c> to construct.
	/// </para>
	/// <para>
	/// <list type="table">
	/// <listheader>
	/// <term>Symbol</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c>!</c></term>
	/// <description>
	/// Will be expanded to the full name of the elements, which is same as <see cref="Type"/>.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>@</c></term>
	/// <description>Will be expanded to the member name, which is same as <see cref="MemberName"/>.</description>
	/// </item>
	/// <item>
	/// <term><c>*</c></term>
	/// <description>Will be expanded to <c>GetEnumerator()</c> invocation.</description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// For example, <c><![CDATA[((IEnumerable<!>)@).*]]></c> will be expanded to the expression
	/// <c><![CDATA[((IEnumerable<ElementType>)MemberName).GetEnumerator()]]></c>.
	/// </para>
	/// </summary>
	[DisallowNull]
	public string? Pattern { get; init; } = "*";

	/// <inheritdoc/>
	public string? MemberName { get; }

	/// <inheritdoc/>
	public Type Type { get; }
}
