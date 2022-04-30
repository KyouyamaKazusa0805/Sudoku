namespace Sudoku.Bot.Oicq.Concepts.Helpers;

/// <summary>
/// Defines an INI object.
/// </summary>
public sealed class IniObject
{
	/// <summary>
	/// Initializes a <see cref="IniObject"/> instance via the local path.
	/// </summary>
	/// <param name="path">The path.</param>
	public IniObject(string path)
	{
		Fileinfo = new FileInfo(path);
		if (!Fileinfo.Exists)
		{
			Fileinfo.Create().Close();
		}
	}


	/// <summary>
	/// Indicates the file information that corresponds to the INI object.
	/// </summary>
	public FileInfo Fileinfo { get; set; }

	/// <summary>
	/// Indicates the sections of the list.
	/// </summary>
	public ICollection<IniSection> Sections { get; set; } = new List<IniSection>();


	/// <summary>
	/// Gets the INI section via the section name.
	/// </summary>
	/// <param name="name">The section name.</param>
	/// <returns>The INI section.</returns>
	public IniSection this[string name]
	{
		get
		{
			var section = ((List<IniSection>)Sections).Find(s => s.SectionName == name);
			if (section is null)
			{
				Sections.Add(section = new IniSection(name));
			}

			return section;
		}
	}


	/// <summary>
	/// Parse the INI string line by line.
	/// </summary>
	public void Load()
	{
		using var stream = Fileinfo.OpenRead();
		var reader = new StreamReader(stream);

		string? line = reader.ReadLine();
		IniSection? section = null;
		while (line is not null)
		{
			// Parse the current line.
			line = line.Trim();

			switch (line)
			{
				case []:
				case ['#', ..]:
				case ['/', '/', ..]:
				{
					break;
				}
				case [var firstCharacter, .., var lastCharacter]:
				{
					if ((firstCharacter, lastCharacter) == ('[', ']'))
					{
						if (section != null)
						{
							// Adds it into the current section.
							Sections.Add(section);

							// Creates a new section.
							section = new IniSection(line, true);
						}
						else
						{
							section = new IniSection(line, true);
						}
					}

					if (line.Split('=') is [var a, var b])
					{
						section!.Values.Add(
							section is not null
								? new(a.Trim(), b.Trim())
								: throw new InvalidOperationException("The key-value pair is found but the section doesn't exist.")
						);
					}

					line = reader.ReadLine();
					break;
				}
			}
		}

		if (section is not null)
		{
			Sections.Add(section);
		}

		reader.Dispose();
		reader.Close();
	}

	/// <summary>
	/// Saves the file.
	/// </summary>
	public void Save()
	{
		using var writer = new StreamWriter(Fileinfo.FullName, false);
		foreach (var item in Sections)
		{
			writer.WriteLine($"[{item.SectionName}]");

			foreach (var pair in item.Values)
			{
				writer.WriteLine($"{pair.Key}={pair.Value}");
			}
		}
	}
}
