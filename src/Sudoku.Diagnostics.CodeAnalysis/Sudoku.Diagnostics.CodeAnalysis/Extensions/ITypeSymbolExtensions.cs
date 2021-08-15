namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="ITypeSymbol"/>.
/// </summary>
/// <seealso cref="ITypeSymbol"/>
internal static class ITypeSymbolExtensions
{
	public static void Deconstruct(
		this ITypeSymbol @this, out bool isValueType, out bool isReferenceType, out bool isNullable)
	{
		isValueType = @this.IsValueType;
		isReferenceType = @this.IsReferenceType;
		isNullable = @this.IsNullableType();
	}

	/// <summary>
	/// Determine whether the current type symbol is a nullable type. The nullable types
	/// are:
	/// <list type="number">
	/// <item>Nullable value type (abbr. NVT), i.e. <see cref="Nullable{T}"/>.</item>
	/// <item>Nullable reference type (abbr. NRT).</item>
	/// </list>
	/// </summary>
	/// <param name="this">The symbol to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks>
	/// This method also checks the nullable state when the type is a nullable reference type.
	/// For example, if the code is like <c>var p = new Class();</c>, p is always corresponds to
	/// nullable type <c>Class?</c>; however, because the initialization clause is a <see langword="new"/>
	/// clause, so the type result will be <c>Class</c> instead of <c>Class?</c>, because current state is
	/// not <see langword="null"/>.
	/// </remarks>
	/// <seealso cref="Nullable{T}"/>
	public static bool IsNullableType(this ITypeSymbol @this) =>
		@this.ToDisplayString() is var str && str[str.Length - 1] == '?';

	/// <summary>
	/// Get all members that belongs to the type and its base types
	/// (but interfaces checks the property <see cref="ITypeSymbol.AllInterfaces"/>).
	/// </summary>
	/// <param name="this">The symbol.</param>
	/// <returns>All members.</returns>
	public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol @this)
	{
		switch (@this)
		{
			case { TypeKind: TypeKind.Interface }:
			{
				foreach (var @interface in @this.AllInterfaces)
				{
					foreach (var member in @interface.GetMembers())
					{
						yield return member;
					}
				}

				goto default;
			}
			default:
			{
				for (var typeSymbol = @this; typeSymbol is not null; typeSymbol = typeSymbol.BaseType)
				{
					foreach (var member in typeSymbol.GetMembers())
					{
						yield return member;
					}
				}

				break;
			}
		}
	}

	/// <summary>
	/// Get all deconstruction methods in this current type.
	/// </summary>
	/// <param name="this">The type.</param>
	/// <returns>All possible deconstruction methods.</returns>
	public static IEnumerable<IMethodSymbol> GetAllDeconstructionMethods(this ITypeSymbol @this) =>
		from method in @this.GetAllMembers().OfType<IMethodSymbol>()
		where method.IsDeconstructionMethod()
		orderby method.Parameters.Length
		select method;
}
