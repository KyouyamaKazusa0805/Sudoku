namespace Sudoku.CodeGenerating;

/// <summary>
/// Used on an assembly, to tell the compiler this assembly will generate a serial of extension methods called
/// <c>Deconstruct</c> that used for deconstruction of an instance.
/// </summary>
/// <remarks>
/// <para>
/// For example, if you write the code like:
/// <code><![CDATA[[assembly: AutoDeconstructExtension<Class>(nameof(Class.A), nameof(Class.B), nameof(Class.C))]]]></code>
/// then you'll get the generated code:
/// <code><![CDATA[
/// public static partial class ClassExtensions
/// {
///     [CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
///     public static void Deconstruct(this Class @this, out int a, out int b, out int c)
///     {
///         a = @this.A;
///         b = @this.B;
///         c = @this.C;
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
[AttributeUsage(Assembly, AllowMultiple = true, Inherited = false)]
public sealed class AutoDeconstructExtensionAttribute<T> : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AutoDeconstructAttribute"/> instance with the members.
	/// </summary>
	/// <param name="memberNames">The member names.</param>
	public AutoDeconstructExtensionAttribute(params string[] memberNames) => MemberNames = memberNames;


	/// <summary>
	/// Indicates the namespace that the output extension class stored. If the value is
	/// <see langword="null"/>, the namespace will use the basic namespace of the type itself.
	/// </summary>
	public string? Namespace { get; init; }

	/// <summary>
	/// Indicates the member names.
	/// </summary>
	public string[] MemberNames { get; }
}
