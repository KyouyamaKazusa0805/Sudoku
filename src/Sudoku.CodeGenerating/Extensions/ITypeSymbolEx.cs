using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGenerating.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ITypeSymbol"/>.
	/// </summary>
	/// <seealso cref="ITypeSymbol"/>
	public static class ITypeSymbolEx
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
	}
}
