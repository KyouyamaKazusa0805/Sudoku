using System;
using System.Runtime.CompilerServices;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="string"/>.
	/// </summary>
	/// <seealso cref="string"/>
	public static class StringEx
	{
		/// <summary>
		/// Converts the current name into the camel case.
		/// </summary>
		/// <param name="this">The name.</param>
		/// <param name="caseConvertingOption">The option that decides the result.</param>
		/// <returns>The result name.</returns>
		public static unsafe string ToCamelCase(
			this string @this, CaseConvertingOption caseConvertingOption = CaseConvertingOption.None)
		{
			switch (@this[0])
			{
				case >= 'A' and <= 'Z':
				{
					char* ptr = stackalloc char[@this.Length];
					fixed (char* pString = @this)
					{
						Unsafe.CopyBlock(ptr, pString, (uint)(sizeof(char) * @this.Length));
					}

					ptr[0] += ' ';

					return new string(ptr);
				}
				case >= 'a' and <= 'z':
				{
					return @this;
				}
				case '_' when @this.Length >= 2 && @this[1] is >= 'A' and <= 'Z' or >= 'a' and <= 'z':
				{
					if (@this[1] is >= 'A' and <= 'Z')
					{
						char* ptr = stackalloc char[@this.Length];
						fixed (char* pString = @this)
						{
							Unsafe.CopyBlock(ptr, pString, (uint)(sizeof(char) * @this.Length));
						}

						ptr[1] += ' ';

						return new string(ptr);
					}
					else
					{
						return caseConvertingOption switch
						{
							CaseConvertingOption.None => @this.Substring(1),
							CaseConvertingOption.ReserveLeadingUnderscore => @this
						};
					}
				}
				default:
				{
					throw new ArgumentException(
						"The specified argument is invalid name to convert.",
						nameof(@this)
					);
				}
			}
		}

		/// <summary>
		/// Converts the current name into the pascal case.
		/// </summary>
		/// <param name="this">The name.</param>
		/// <returns>The result name.</returns>
		public static unsafe string ToPascalCase(this string @this)
		{
			switch (@this[0])
			{
				case >= 'A' and <= 'Z':
				{
					return @this;
				}
				case >= 'a' and <= 'z':
				{
					char* ptr = stackalloc char[@this.Length];
					Unsafe.InitBlock(ptr, 0, (uint)(sizeof(char) * @this.Length));

					fixed (char* pString = @this)
					{
						Unsafe.CopyBlock(ptr, pString, (uint)(sizeof(char) * @this.Length));
					}

					ptr[0] -= ' ';

					return new string(ptr);
				}
				case '_' when @this.Length >= 2 && @this[1] is >= 'A' and <= 'Z' or >= 'a' and <= 'z':
				{
					char* ptr = stackalloc char[@this.Length - 1];
					Unsafe.InitBlock(ptr, 0, (uint)(sizeof(char) * (@this.Length - 1)));

					fixed (char* pString = @this)
					{
						Unsafe.CopyBlock(ptr, pString + 1, (uint)(sizeof(char) * (@this.Length - 1)));
					}

					if (ptr[0] is >= 'a' and <= 'z')
					{
						ptr[0] -= ' ';
					}

					return new string(ptr);
				}
				default:
				{
					throw new ArgumentException(
						"The specified argument is invalid name to convert.",
						nameof(@this)
					);
				}
			}
		}
	}
}
