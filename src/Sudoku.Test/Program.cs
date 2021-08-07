using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Linq;
using System.Reflection;
using System.Text;
using Sudoku.Solving.Manual;

string format = "Hello, {WorldText}{{{Next}}}";
string r = f(format, typeof(S), new S());

Console.WriteLine(format);
Console.WriteLine(r);

static string f(string @this, Type type, object obj)
{
	if ((@this.CountOf('{') & 1) != 0 || (@this.CountOf('}') & 1) != 0)
	{
		throw new ArgumentException(
			"The format is invalid. The number of all curly brace characters should be an even.",
			nameof(@this)
		);
	}

	int length = @this.Length;
	var sb = new StringBuilder(length);
	var formats = new List<string>();
	int formatCount = 0;
	for (int i = 0, iterationLength = length - 1; i < iterationLength; i++)
	{
		switch ((Left: @this[i], Right: @this[i + 1]))
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
					if (@this[j] == '}')
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
				formats.Add(@this[(i + 1)..pos]);

				i = pos;

				break;
			}
			case (Left: var left, _):
			{
				sb.Append(left);

				break;
			}
		}
	}

	const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
	string[] matchedFormats = (
		from format in formats
		select type.GetProperty(format, flags) into property
		where property is not null
		where property.GetCustomAttribute<FormatItemAttribute>() is not null
		select property.GetValue(obj) as string into result
		where result is not null
		select result
	).ToArray();

	if (formatCount != matchedFormats.Length)
	{
		throw new ArgumentException("The format is invalid. The number of interpolations failed to match.");
	}

	return string.Format(sb.ToString(), matchedFormats);
}


class S
{
	[FormatItem]
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicMethods)]
	private string WorldText => "world";

	[FormatItem]
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicMethods)]
	private string Next => "!";
}