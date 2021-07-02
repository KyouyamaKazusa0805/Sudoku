using System;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Used on an assembly, to tell the compiler this assembly will generate
	/// a extension method called <c>Deconstruct</c>.
	/// </summary>
	/// <remarks>
	/// For example, if you write the code like:
	/// <code>
	/// [assembly: AutoDeconstructExtension(typeof(Class), nameof(Class.A), nameof(Class.B), nameof(Class.C))]
	/// </code>
	/// then you'll get the generated code:
	/// <code>
	/// using System.Runtime.CompilerServices;
	/// 
	/// public static class ClassEx
	/// {
	///     [CompilerGenerated]
	///     [MethodImpl(MethodImplOptions.AggressiveInlining)]
	///     public static void Deconstruct(this Class @this, out int a, out int b, out int c)
	///     {
	///         a = @this.A;
	///         b = @this.B;
	///         c = @this.C;
	///     }
	/// }
	/// </code>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class AutoDeconstructExtensionAttribute : Attribute
	{
		/// <summary>
		/// Initializes an <see cref="AutoDeconstructAttribute"/> instance with the specified type
		/// and the members.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="memberNames">The member names.</param>
		public AutoDeconstructExtensionAttribute(Type type, params string[] memberNames)
		{
			TypeToGenerate = type;
			MemberNames = memberNames;
		}


		/// <summary>
		/// Indicates the namespace that the output extension class stored. If the value is
		/// <see langword="null"/>, the namespace will use the basic namespace of the type itself.
		/// </summary>
		public string? Namespace { get; init; }

		/// <summary>
		/// Indicates the member names.
		/// </summary>
		public string[] MemberNames { get; }

		/// <summary>
		/// Indicates the type to generate the "<c>Deconstruct</c>" method.
		/// </summary>
		public Type TypeToGenerate { get; }
	}
}
