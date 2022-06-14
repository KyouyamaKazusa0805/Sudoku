namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attributes that allows user using it applying to a type
/// (especially for <see langword="class"/> or <see langword="struct"/>), indicating the overridden metadata
/// for method <see cref="object.ToString"/>, and make source generator generates their own <c>ToString</c>
/// method automatically.
/// </summary>
/// <remarks>
/// At default, the generated string value will be the concatenation of assignment styled string with key-value pairs.
/// For example, if the attribute references two properties in the type:
/// <code><![CDATA[
/// [AutoOverridesToString(nameof(A), nameof(B))]
/// public sealed class ExampleType
/// {
///     public int A { get; } = 42;
///     public int B { get; } = 108;
/// }
/// ]]></code>
/// The generated output string will be:
/// <code><![CDATA[
/// ExampleType { A = 42, B = 108 }
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class AutoOverridesToStringAttribute :
	SourceGeneratorOptionProviderAttribute,
	IMultipleMembersBinder,
	IPatternProvider
{
	/// <summary>
	/// Initializes an <see cref="AutoOverridesToStringAttribute"/> instance via the specified array
	/// of <see cref="string"/> elements indicating the names of the data members you want to be output
	/// in the output source file.
	/// </summary>
	/// <param name="memberExpressions">The name of data members, represented as a <see cref="string"/> array.</param>
	public AutoOverridesToStringAttribute(params string[] memberExpressions) => MemberExpressions = memberExpressions;


	/// <summary>
	/// <para>
	/// Indicates the pattern that describes the final output string.
	/// For example, if the current property is <see langword="null"/>, the source generator will emit
	/// type name at the first place, then property-value pairs list.
	/// </para>
	/// <para>
	/// You can use the following symbols to help you define your own pattern:
	/// <list type="table">
	/// <listheader>
	/// <term>Symbol</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c>[index]</c>, where <c>index</c> is an index value beginning from 0</term>
	/// <description>The member name at the specified index stored in the property <see cref="MemberExpressions"/></description>
	/// </item>
	/// <item>
	/// <term><c>*</c></term>
	/// <description>Will be expanded to the code <c>ToString()</c></description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// For example, the pattern <c>((char)[0]).*</c> means the first element in the array <see cref="MemberExpressions"/>
	/// will be converted into a <see cref="char"/>, and then invoke the method <see cref="char.ToString()"/>
	/// to get the result value.
	/// </para>
	/// <para>The default value is <see langword="null"/>.</para>
	/// </summary>
	[DisallowNull]
	public string? Pattern { get; init; }

	/// <inheritdoc/>
	public string[] MemberExpressions { get; }
}
