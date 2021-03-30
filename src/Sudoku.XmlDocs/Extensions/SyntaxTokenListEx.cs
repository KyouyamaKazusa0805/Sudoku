using System;
using System.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SyntaxTokenList"/>.
	/// </summary>
	/// <seealso cref="SyntaxTokenList"/>
	public static class SyntaxTokenListEx
	{
		/// <summary>
		/// Get custom modifiers from the specified syntax token list.
		/// </summary>
		/// <param name="this">The list.</param>
		/// <returns>The result modifiers.</returns>
		public static CustomModifier GetCustomModifiers(this in SyntaxTokenList @this)
		{
			var result = CustomModifier.None;
			foreach (var modifierToCheck in CustomModifier.All)
			{
				if (@this.Contains(modifierToCheck))
				{
					result |= modifierToCheck;
				}
			}

			return result;
		}

		/// <summary>
		/// Get custom accessibilities from the specified syntax token list.
		/// </summary>
		/// <param name="this">The list.</param>
		/// <returns>The result accessibilities.</returns>
		public static CustomAccessibility GetCustomAccessibilities(this in SyntaxTokenList @this)
		{
			var result = CustomAccessibility.None;
			foreach (var token in @this)
			{
				result |= token switch
				{
					{ RawKind: (int)SyntaxKind.PublicKeyword } => CustomAccessibility.Public,
					{ RawKind: (int)SyntaxKind.InternalKeyword } => CustomAccessibility.Internal,
					{ RawKind: (int)SyntaxKind.ProtectedKeyword } => CustomAccessibility.Protected,
					{ RawKind: (int)SyntaxKind.PrivateKeyword } => CustomAccessibility.Private,
				};
			}

			return result;
		}

		/// <summary>
		/// Check whether the specified list contains the specified modifier.
		/// </summary>
		/// <param name="this">The list.</param>
		/// <param name="customModifier">The custom modifier.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool Contains(this in SyntaxTokenList @this, CustomModifier customModifier)
		{
			string modifierName = customModifier.ToString().ToLower();
			foreach (var token in @this)
			{
				if (token.ValueText == modifierName)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check whether the specified list contains the specified accessibility.
		/// </summary>
		/// <param name="this">The list.</param>
		/// <param name="customAccessibility">The custom accessibility.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool Contains(this in SyntaxTokenList @this, CustomAccessibility customAccessibility)
		{
			var syntaxKinds = customAccessibility switch
			{
				CustomAccessibility.Public => new[] { SyntaxKind.PublicKeyword },
				CustomAccessibility.Internal => new[] { SyntaxKind.InternalKeyword },
				CustomAccessibility.ProtectedInternal => new[]
				{
					SyntaxKind.ProtectedKeyword,
					SyntaxKind.InternalKeyword
				},
				CustomAccessibility.Protected => new[] { SyntaxKind.ProtectedKeyword },
				CustomAccessibility.Private => new[] { SyntaxKind.PrivateKeyword },
				CustomAccessibility.PrivateProtected => new[]
				{
					SyntaxKind.PrivateKeyword,
					SyntaxKind.ProtectedKeyword
				}
			};

			foreach (var token in @this)
			{
				var syntaxToken = token;
				if (Array.Exists(syntaxKinds, syntaxKind => syntaxToken.IsKind(syntaxKind)))
				{
					return true;
				}
			}

			return false;
		}
	}
}
