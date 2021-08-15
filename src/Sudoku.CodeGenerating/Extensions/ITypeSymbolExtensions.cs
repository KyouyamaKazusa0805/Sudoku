namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="ITypeSymbol"/>.
/// </summary>
/// <seealso cref="ITypeSymbol"/>
internal static class ITypeSymbolExtensions
{
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
	/// Get the detail information that represented as <see cref="string"/> values.
	/// </summary>
	/// <param name="this">The symbol.</param>
	/// <param name="fullTypeName">The full type name.</param>
	/// <param name="namespaceName">The namespace name.</param>
	/// <param name="genericParametersList">
	/// The generic parameter list. The type parameter constraint will also include,
	/// e.g. <c><![CDATA[Namespace.TypeName<T> where T : class?]]></c>.
	/// </param>
	/// <param name="genericParametersListWithoutConstraint">
	/// The generic parameter list without type parameter constraint,
	/// e.g. <c><![CDATA[Namespace.TypeName<T>]]></c>.
	/// </param>
	/// <param name="fullTypeNameWithoutConstraint">The full type name without constraint.</param>
	/// <param name="constraint">The constraint.</param>
	/// <param name="isGeneric">Indicates whether the type is a generic type.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DeconstructInfo(
		this ITypeSymbol @this, out string fullTypeName, out string namespaceName,
		out string genericParametersList, out string genericParametersListWithoutConstraint,
		out string fullTypeNameWithoutConstraint, out string constraint,
		out bool isGeneric)
	{
		fullTypeName = @this.ToDisplayString(FormatOptions.TypeFormat);
		namespaceName = @this.ContainingNamespace.ToDisplayString();

		int i = fullTypeName.IndexOf('<');
		isGeneric = i != -1;
		genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);

		int j = fullTypeName.IndexOf('>');
		genericParametersListWithoutConstraint = i == -1 ? string.Empty : fullTypeName.Substring(i, j - i + 1);

		fullTypeNameWithoutConstraint = j + 1 == genericParametersList.Length ? fullTypeName : fullTypeName.Substring(0, j);
		constraint = j + 1 == genericParametersList.Length ? string.Empty : genericParametersList.Substring(j + 1);
	}
}
