namespace Sudoku.Solving;

/// <summary>
/// Provides with a manual solving step that is a technique usage, and contains the conclusions.
/// </summary>
/// <param name="Conclusions"><inheritdoc cref="IVisual.Conclusions" path="/summary"/></param>
/// <param name="Views"><inheritdoc cref="IVisual.Views" path="/summary"/></param>
internal abstract record Step(ImmutableArray<Conclusion> Conclusions, ImmutableArray<View> Views) : IStep
{
	/// <inheritdoc/>
	public virtual string Name => R[TechniqueCode.ToString()]!;

	/// <inheritdoc/>
	public virtual string? Format => R[$"TechniqueFormat_{GetType().Name}"];

	/// <inheritdoc/>
	public abstract decimal Difficulty { get; }

	/// <inheritdoc/>
	public abstract Technique TechniqueCode { get; }

	/// <inheritdoc/>
	public abstract TechniqueTags TechniqueTags { get; }

	/// <inheritdoc/>
	public virtual TechniqueGroup TechniqueGroup
		=> Enum.TryParse<TechniqueGroup>(TechniqueCode.ToString(), out var inst) ? inst : TechniqueGroup.None;

	/// <inheritdoc/>
	public abstract DifficultyLevel DifficultyLevel { get; }

	/// <inheritdoc/>
	public virtual Stableness Stableness => Stableness.Stable;

	/// <inheritdoc/>
	public abstract Rarity Rarity { get; }


	/// <inheritdoc/>
	public void ApplyTo(scoped ref Grid grid)
	{
		foreach (var conclusion in Conclusions)
		{
			conclusion.ApplyTo(ref grid);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasTag(TechniqueTags flags)
		=> flags.IsFlag() ? TechniqueTags.Flags(flags) : TechniqueTags.MultiFlags(flags);

	/// <summary>
	/// Returns a string that only contains the name and the basic information.
	/// </summary>
	/// <returns>The string instance.</returns>
	/// <remarks>
	/// <para>
	/// This method uses <see langword="sealed"/> and <see langword="override"/> modifiers
	/// to prevent the compiler overriding the method.
	/// </para>
	/// <para>The behavior is same as the method <see cref="Formatize(bool)"/> invoking.</para>
	/// </remarks>
	/// <seealso cref="Formatize(bool)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString() => Formatize(true);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString() => $"{Name} => {ElimStr()}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual string ToFullString() => ToString();

	/// <inheritdoc/>
	public string Formatize(bool handleEscaping = false)
	{
		// Check whether the format property is not null.

		if (Format is not { } format)
		{
			throw new InvalidOperationException("The format can't be null.");
		}

		// Get the interpolation values, and extract them into a new collection to store the format values.
		int length = format.Length;
		scoped var sb = new StringHandler(length);
		var formats = new List<string>(10);
		int formatCount = 0;
		for (int i = 0, iterationLength = length - 1; i < iterationLength; i++)
		{
			switch (format[i], format[i + 1])
			{
				case ('{', '}'):
				{
					throw new InvalidOperationException("The interpolation part cannot contain empty value.");
				}
				case ('{', '{'):
				{
					sb.Append("{{");
					i++;

					break;
				}
				case ('}', '}'):
				{
					sb.Append("}}");
					i++;

					break;
				}
				case ('{', not '{'):
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
						throw new InvalidOperationException("Missing the closed brace character '}'.");
					}

					sb.Append('{');
					sb.Append(formatCount++);
					sb.Append('}');

					formats.Add(format[(i + 1)..pos]);

					i = pos;

					break;
				}
				case ('\\', var right) when handleEscaping: // Unescape the escaping characters.
				{
					sb.Append(right);
					i++;

					break;
				}
				case (var left, _):
				{
					sb.Append(left);

					break;
				}
			}
		}

		// Use reflection to invoke each properties, and get the interpolation result.
		var type = GetType();
		string[] matchedFormats = (
			from f in formats
			select type.GetMethod(f, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance) into method
			where method?.IsDefined(typeof(ResourceTextFormatterAttribute)) ?? false
			select method.Invoke(method.IsStatic ? null : this, null) as string into result
			where result is not null
			select result
		).Prepend(Name).ToArray();

		// Check the length validity.
		if (formatCount != matchedFormats.Length)
		{
			throw new InvalidOperationException("The number of interpolations failed to match.");
		}

		// Format and return the value.
		return string.Format(sb.ToStringAndClear(), matchedFormats);
	}

	[ResourceTextFormatter]
	protected string ElimStr() => ConclusionFormatter.Format(Conclusions.ToArray(), FormattingMode.Normal);

	/// <inheritdoc/>
	string IStep.ElimStr() => ElimStr();
}
