namespace Sudoku.Bot.Oicq.Concepts.Helpers;

/// <summary>
/// Indicates an INI section.
/// </summary>
/// <param name="SectionName">Indicates the name of the section.</param>
/// <param name="Values">Indicates the values in this section.</param>
public sealed record class IniSection(string SectionName, IList<KeyValuePair<string, string>> Values)
{
	/// <summary>
	/// Initializes an <see cref="IniSection"/> instance via the name.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="hasTrimmedSquareBracketsToken">
	/// Indicates whether the name has been trimmed the square bracket token <c>'['</c> and <c>']'</c>.
	/// The default value is <see langword="false"/>.
	/// </param>
	public IniSection(string name, bool hasTrimmedSquareBracketsToken = false) :
		this(name, new List<KeyValuePair<string, string>>())
	{
		if (hasTrimmedSquareBracketsToken)
		{
			SectionName = SectionName.TrimStart('[').TrimEnd(']');
		}
	}


	/// <summary>
	/// Gets the value via the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>The corresponding value.</returns>
	public string this[string key]
	{
		get
		{
			var pair = ((List<KeyValuePair<string, string>>)Values).Find(p => p.Key == key);
			if (pair.Equals(default(KeyValuePair<string, string>)))
			{
				pair = new KeyValuePair<string, string>(key, string.Empty);
				Values.Add(pair);
			}

			return pair.Value;
		}

		set
		{
			int index = ((List<KeyValuePair<string, string>>)Values).FindIndex(p => p.Key == key);
			if (index == -1)
			{
				var pair = new KeyValuePair<string, string>(key, value);
				Values.Add(pair);
			}
			else
			{
				Values[index] = new KeyValuePair<string, string>(key, value);
			}
		}
	}
}
