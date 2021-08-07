using System;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Reflection;
using System.Text;
using Sudoku.Solving.Manual;

namespace Sudoku.Windows.Extensions
{
	/// <summary>
	/// Provides extension methods on generic argument.
	/// </summary>
	internal static class GenericEx
	{
		/// <summary>
		/// Formatizes the <see cref="StepInfo.Format"/> property string and output the result.
		/// </summary>
		/// <param name="this">The object to calls the reflection.</param>
		/// <param name="handleEsapcing">Indicates whether the method will handle the escaping characters.</param>
		/// <returns>The result string.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when the format is invalid. The possible cases are:
		/// <list type="bullet">
		/// <item>The format is null.</item>
		/// <item>The interpolation part contains the empty value.</item>
		/// <item>Missing the closed brace character <c>'}'</c>.</item>
		/// <item>The number of interpolations failed to match.</item>
		/// </list>
		/// </exception>
		/// <exception cref="NullReferenceException">
		/// Throws when the string handling encounters a <see langword="null"/> reference.
		/// </exception>
		/// <seealso cref="StepInfo.Format"/>
		public static string Formatize(this StepInfo @this, bool handleEsapcing = false)
		{
			// Check whether the format property is not null.
			if (@this.Format is not { } format)
			{
				throw new ArgumentException("The format can't be null.", nameof(@this));
			}

			// Get the interpolation values, and extract them into a new collection to store the format values.
			int length = format.Length;
			var sb = new StringBuilder(length);
			var formats = new List<string>();
			int formatCount = 0;
			for (int i = 0, iterationLength = length - 1; i < iterationLength; i++)
			{
				switch ((Left: format[i], Right: format[i + 1]))
				{
					case (Left: '{', Right: '}'):
					{
						throw new ArgumentException(
							"The format is invalid. The interpolation part cannot contain empty value.",
							nameof(@this)
						);
					}
					case (Left: '{', Right: '{'):
					{
						sb.Append("{{");
						i++;

						break;
					}
					case (Left: '}', Right: '}'):
					{
						sb.Append("}}");
						i++;

						break;
					}
					case (Left: '{', Right: not '{'):
					{
						int pos = -1;
						for (int j = i + 1; j < length; j++)
						{
							if (format[j] == '}')
							{
								pos = j;
								break;
							}
						}
						if (pos == -1)
						{
							throw new ArgumentException(
								"The format is invalid. Missing the closed brace character '}'.",
								nameof(@this)
							);
						}

						sb.Append('{').Append(formatCount++).Append('}');
						formats.Add(format[(i + 1)..pos]);

						i = pos;

						break;
					}
					case (Left: '\\', Right: var right) when handleEsapcing: // De-escape the escaping characters.
					{
						sb.Append(right);
						i++;

						break;
					}
					case (Left: var left, _):
					{
						sb.Append(left);

						break;
					}
				}
			}

			// Use reflection to invoke each properties, and get the interpolation result.
			var type = @this.GetType();
			const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
			string[] matchedFormats = (
				from f in formats
				select type.GetProperty(f, flags) into property
				where property?.GetCustomAttribute<FormatItemAttribute>() is not null
				let isPropStatic = property.GetMethod?.IsStatic
				where isPropStatic is not null
				select property.GetValue(isPropStatic.Value ? null : @this) as string into result
				where result is not null
				select result
			).Prepend(@this.Name).ToArray();

			// Check the length validity.
			if (formatCount != matchedFormats.Length)
			{
				throw new ArgumentException(
					"The format is invalid. The number of interpolations failed to match.",
					nameof(@this)
				);
			}

			// Format and return the value.
			return string.Format(sb.ToString(), matchedFormats);
		}
	}
}
