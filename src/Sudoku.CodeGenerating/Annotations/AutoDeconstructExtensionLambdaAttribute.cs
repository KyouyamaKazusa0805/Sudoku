namespace Sudoku.CodeGenerating;

/// <summary>
/// Mark this attribute onto a type, to tell the source generator that the source generator will
/// generates the extension deconstruction methods, with expressions.
/// </summary>
/// <typeparam name="T">
/// The type of the element as the type of the generated code for the deconstruction methods.
/// </typeparam>
/// <typeparam name="TProvider">
/// The type of the provider class that provides the extension methods for the argument provider
/// of extension deconstruction methods.
/// </typeparam>
/// <remarks>
/// <para>
/// For example, if you write the code like:
/// <code><![CDATA[
/// [assembly: AutoDeconstructExtensionLambda<Class, Provider>(nameof(Class.A), nameof(Class.B), $".{nameof(Provider.C)}")]
/// ]]></code>
/// (Real arguments are <c>"A"</c>, <c>"B"</c> and <c>".C"</c>.)
/// </para>
/// <para>
/// We suppose that the value <c>.C</c> is an expression that is defined by ours:
/// <code><![CDATA[
/// [PrivatizeParameterlessConstructor]
/// internal sealed class Provider
/// {
///		[DeconstructArgumentProvider]
///     internal static int C(Class instance) => instance.A + instance.B;
/// }
/// ]]></code>
/// then you'll get the generated code:
/// <code><![CDATA[
/// public static partial class ClassExtensions
/// {
///     [CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
///     public static void Deconstruct(this Class @this, out int a, out int b, out int c)
///     {
///         a = @this.A;
///         b = @this.B;
///         c = Provider.C(@this); // Translated into '@this.A + @this.B' here.
///     }
/// }
/// ]]></code>
/// </para>
/// <para>
/// Please note that this attribute is a generic attribute, which is introduced in C# 10.
/// If you don't know this feature (generic attribute), please visit
/// <see href="https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/generic-attributes.md">
/// this link
/// </see>
/// for more information.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class AutoDeconstructExtensionLambdaAttribute<T, TProvider> : Attribute where TProvider : class
{
	/// <summary>
	/// Initializes an <see cref="AutoDeconstructLambdaAttribute"/> instance via the member names.
	/// </summary>
	/// <param name="memberNames">The member names.</param>
	public AutoDeconstructExtensionLambdaAttribute(params string[] memberNames) => MemberNames = memberNames;


	/// <summary>
	/// Indicates the namespace that the output extension class stored. If the value is
	/// <see langword="null"/>, the namespace will use the basic namespace of the type itself.
	/// </summary>
	public string? Namespace { get; init; }

	/// <summary>
	/// Indicates the member names to deconstruct.
	/// </summary>
	public string[] MemberNames { get; }
}
