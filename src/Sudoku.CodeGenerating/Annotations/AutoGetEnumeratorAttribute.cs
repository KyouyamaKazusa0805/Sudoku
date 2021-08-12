namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// To mark on a type, to tell the compiler that the compiler will automatically generate
	/// <c>GetEnumerator</c> methods for that type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class AutoGetEnumeratorAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with the specified member name.
		/// </summary>
		/// <param name="memberName">
		/// The member name. If you want to pass "this" as the reference, just input "<c>@</c>".
		/// </param>
		public AutoGetEnumeratorAttribute(string memberName) => MemberName = memberName;


		/// <summary>
		/// Indicates the member name.
		/// </summary>
		public string MemberName { get; }

		/// <summary>
		/// Indicates the member conversion that is used for creation of the enumerator.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property should be stored a lambda body. One of possible syntaxes is:
		/// <code><![CDATA[
		/// [AutoGetEnumerator(nameof(MemberName), "((IEnumerable<int>)@).*")]
		/// ]]></code>
		/// Where:
		/// <list type="table">
		/// <item>
		/// <term>'<c>@</c>'</term>
		/// <description>Equivalent to the member.</description>
		/// </item>
		/// <item>
		/// <term>'<c>*</c>'</term>
		/// <description>Equivalent to the code <c>GetEnumerator()</c>.</description>
		/// </item>
		/// </list>
		/// </para>
		///	<para>
		///	The default conversion is "<c>@</c>".
		/// </para>
		/// </remarks>
		public string MemberConversion { get; init; } = "@";

		/// <summary>
		/// Indicates the extra namespace should be imported.
		/// </summary>
		public string[]? ExtraNamespaces { get; init; }

		/// <summary>
		/// Indicates the return type. If <see langword="null"/>, the value of type <see cref="IEnumerable{T}"/>
		/// of <see cref="int"/> will be returned.
		/// </summary>
		/// <seealso cref="IEnumerable{T}"/>
		public Type? ReturnType { get; init; }
	}
}
